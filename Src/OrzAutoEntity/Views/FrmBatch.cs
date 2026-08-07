using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OrzAutoEntity.DataAccess;
using OrzAutoEntity.Helpers;
using OrzAutoEntity.Modes;
using TreeNode = OrzAutoEntity.Modes.TreeNode;

namespace OrzAutoEntity.Views
{
    public partial class FrmBatch : Form
    {
        private DisplayHandle displayHandle = new DisplayHandle();
        private List<TemplateConfig> templateConfigs;
        private DatabaseConfig dbConfig;
        private DatabaseType dbType;
        private Database db;
        private List<TableInfo> tables;
        private bool isReset;

        public FrmBatch()
        {
            InitializeComponent();

            Text = $"实体类生成工具 {GetVersion()}";
        }

        private void FrmBatch_Load(object sender, EventArgs e)
        {
            isReset = false;
            cbDatabase.Items.Clear();
            ConfigHelper.Databases.ForEach(t => cbDatabase.Items.Add(t));
            var selected = ConfigHelper.Databases.FirstOrDefault(t => t.IsSelected) ?? ConfigHelper.Databases.FirstOrDefault();
            cbDatabase.SelectedItem = selected;
            if (isReset == false)
            {
                InternalReset();
            }
        }

        #region 加载表数据
        private void cbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            InternalReset();

            dbConfig = (DatabaseConfig)cbDatabase.SelectedItem;
            if (Enum.TryParse(dbConfig.Type, out dbType) == false)
            {
                ShowError($"非法的数据库类型：{dbConfig.Type}");
                return;
            }

            templateConfigs = ConfigHelper.GetTemplateConfig(dbConfig.TemplateIds);
            var notExistIds = dbConfig.TemplateIds.Except(templateConfigs.Select(t => t.Id));
            if (notExistIds.Any())
            {
                ShowError($"未找到 id 为 [{string.Join(",", notExistIds)}] 的实体模板");
                return;
            }

            LoadTableInfo();
        }

        private void InternalReset()
        {
            SetButtonEnabled(false);
            updateList.DataSource = null;
            newList.DataSource = null;
            deleteList.Items.Clear();
            txtUpdateFilter.Text = "";
            txtNewFilter.Text = "";
            txtDeleteFilter.Text = "";
            lbConnString.Text = "";
            isReset = true;
        }

        private void SetButtonEnabled(bool enabled)
        {
            btnDelete.Enabled = enabled;
            btnRefresh.Enabled = enabled;
        }

        private void LoadTableInfo()
        {
            try
            {
                var tableFilterConfigs = ConfigHelper.GetFilterConfig(dbConfig.FilterIds, FilterType.Table);
                db = DatabaseFactory.GetDatabase(dbConfig.ConnString, dbType);
                tables = db.GetTableInfos();
                FilterConfig.Handle(tableFilterConfigs, tables);
                var tableFiles = TableFileInfo.GetTableFileInfos(templateConfigs, tables);

                var existsEntities = DTEHelper.GetExistsEntities(dbConfig.Directory);
                var modelFilterConfigs = ConfigHelper.GetFilterConfig(dbConfig.FilterIds, FilterType.Model);
                FilterConfig.Handle(modelFilterConfigs, existsEntities);

                displayHandle.Init(tableFiles, existsEntities);

                //已生成的实体
                updateList.DataSource = displayHandle.FilterUpdateList();
                updateList.DisplayMember = "FilePath";


                //未生成的实体
                newList.DataSource = displayHandle.FilterNewList();
                newList.DisplayMember = "FilePath";

                //数据库中不存在的实体
                foreach (var entity in displayHandle.FilterDeleteList())
                {
                    deleteList.Items.Add(entity);
                }

                SetButtonEnabled(true);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        #endregion

        #region 过滤
        private void txtUpdateFilter_TextChanged(object sender, EventArgs e)
        {
            updateList.DataSource = displayHandle.FilterUpdateList(txtUpdateFilter.Text);
            updateList.DisplayMember = "FilePath";
        }

        private void txtNewFilter_TextChanged(object sender, EventArgs e)
        {
            newList.DataSource = displayHandle.FilterNewList(txtNewFilter.Text);
            newList.DisplayMember = "FilePath";
        }

        private void txtDeleteFilter_TextChanged(object sender, EventArgs e)
        {
            deleteList.Items.Clear();
            foreach (var entity in displayHandle.FilterDeleteList(txtDeleteFilter.Text))
            {
                deleteList.Items.Add(entity);
            }
        }
        #endregion

        #region 全选/全取消
        private void btnAllUpdate_Click(object sender, EventArgs e)
        {
            SetItemChecked(updateList, true);
        }

        private void btnAllNew_Click(object sender, EventArgs e)
        {
            SetItemChecked(newList, true);
        }

        private void btnAllDelete_Click(object sender, EventArgs e)
        {
            SetItemChecked(deleteList, true);
        }

        private void btnCancelUpdate_Click(object sender, EventArgs e)
        {
            SetItemChecked(updateList, false);
        }

        private void btnCancelNew_Click(object sender, EventArgs e)
        {
            SetItemChecked(newList, false);
        }

        private void btnCancelDelete_Click(object sender, EventArgs e)
        {
            SetItemChecked(deleteList, false);
        }

        private void SetItemChecked(CheckedListBox clb, bool isChecked)
        {
            for (int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemChecked(i, isChecked);
            }
        }
        #endregion

        #region 删除实体类
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var fileNames = GetCheckedItems<string>(deleteList);
            if (fileNames.Count == 0)
            {
                ShowError("请勾选要删除的项");
                return;
            }

            SetControlEnabled(this, false);

            var infos = fileNames.Select(t => new TableFileInfo { FilePath = t }).ToList();
            var treeNode = TreeNode.CreateTreeNode(dbConfig, infos);
            treeNode.RemoveFromProject(DTEHelper.GetSelectedProject());

            displayHandle.RemoveDeleteList(fileNames);
            RemoveCheckedItems(deleteList);

            SetControlEnabled(this, true);
        }

        private void RemoveCheckedItems(CheckedListBox clb)
        {
            for (int i = clb.CheckedItems.Count - 1; i > -1; i--)
            {
                clb.Items.Remove(clb.CheckedItems[i]);
            }
        }
        #endregion

        #region 生成实体类
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var updateItems = GetCheckedItems<TableFileInfo>(updateList);
            var newItems = GetCheckedItems<TableFileInfo>(newList);
            updateItems.AddRange(newItems);

            if (updateItems.Count == 0)
            {
                ShowError("请勾选要添加/刷新的项");
                return;
            }

            GenerateFile(updateItems);
        }

        private void GenerateFile(List<TableFileInfo> refreshItems)
        {
            try
            {
                SetControlEnabled(this, false);

                var fillTables = refreshItems.Select(t => t.Table).Distinct().ToList();
                db.FillColumnInfos(fillTables);
                foreach (var item in ConfigHelper.GetColumnConfig(dbConfig.ColumnIds))
                {
                    item.Handle(fillTables);
                }

                var project = DTEHelper.GetSelectedProject();
                var projectFullPath = DTEHelper.GetProjectFullPath(project);
                var treeNode = TreeNode.CreateTreeNode(dbConfig, refreshItems);
                treeNode.GenerateFile(projectFullPath);
                treeNode.AddToProject(project, projectFullPath);

                Close();
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
            finally
            {
                SetControlEnabled(this, true);
            }
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"V{version.Major}.{version.Minor}";
        }

        private void ShowError(string msg)
        {
            MessageBox.Show(msg, "错误提示");
        }

        private List<T> GetCheckedItems<T>(CheckedListBox clb)
        {
            var list = new List<T>();
            for (int i = 0; i < clb.Items.Count; i++)
            {
                if (clb.GetItemChecked(i)) list.Add((T)clb.Items[i]);
            }
            return list;
        }

        private void SetControlEnabled(Control control, bool enabled)
        {
            control.Enabled = enabled;
            foreach (Control item in control.Controls)
            {
                SetControlEnabled(item, enabled);
            }
        }
    }
}
