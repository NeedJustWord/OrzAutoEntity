using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OrzAutoEntity.DataAccess;
using OrzAutoEntity.Helpers;
using OrzAutoEntity.Modes;
using OrzAutoEntity.Services;

namespace OrzAutoEntity.Views
{
    public partial class FrmBatch : Form
    {
        private List<string> updateEntityList = new List<string>();
        private List<string> newEntityList = new List<string>();
        private List<string> deleteEntityList = new List<string>();
        private DatabaseConfig dbConfig;
        private DatabaseType dbType;
        private Database db;
        private List<TableInfo> tables;

        public FrmBatch()
        {
            InitializeComponent();
        }

        private void FrmBatch_Load(object sender, EventArgs e)
        {
            Text = $"实体类生成工具 {GetVersion()}";
            cbDatabase.DisplayMember = "Name";
        }

        public void Reset()
        {
            InternalReset();
            cbDatabase.Items.Clear();
            ConfigHelper.Databases.ForEach(t => cbDatabase.Items.Add(t));
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

            LoadTableInfo();
        }

        private void InternalReset()
        {
            SetButtonEnabled(false);
            updateList.Items.Clear();
            newList.Items.Clear();
            deleteList.Items.Clear();
            updateEntityList.Clear();
            newEntityList.Clear();
            deleteEntityList.Clear();
            txtUpdateFilter.Text = "";
            txtNewFilter.Text = "";
            txtDeleteFilter.Text = "";
            lbConnString.Text = "";
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
                var filterConfig = ConfigHelper.GetFilterConfig(dbConfig.FilterId);
                db = DatabaseFactory.GetDatabase(dbConfig.ConnString, dbType);
                tables = db.GetTableInfos().Filter(filterConfig);

                var tableNames = tables.Select(t => t.Name).ToList();
                var existsEntities = DTEHelper.GetExistsEntities(dbConfig.Directory);

                //已生成的实体
                foreach (var entity in tableNames.Intersect(existsEntities))
                {
                    updateList.Items.Add(entity);
                    updateEntityList.Add(entity);
                }

                //未生成的实体
                foreach (var entity in tableNames.Except(existsEntities))
                {
                    newList.Items.Add(entity);
                    newEntityList.Add(entity);
                }

                //数据库中不存在的实体
                foreach (var entity in existsEntities.Except(tableNames))
                {
                    deleteList.Items.Add(entity);
                    deleteEntityList.Add(entity);
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
            FilterList(txtUpdateFilter, updateList, updateEntityList);
        }

        private void txtNewFilter_TextChanged(object sender, EventArgs e)
        {
            FilterList(txtNewFilter, newList, newEntityList);
        }

        private void txtDeleteFilter_TextChanged(object sender, EventArgs e)
        {
            FilterList(txtDeleteFilter, deleteList, deleteEntityList);
        }

        private void FilterList(TextBox tb, CheckedListBox clb, List<string> data)
        {
            var input = tb.Text;
            var result = data.Where(t => t.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1);
            clb.Items.Clear();
            foreach (var item in result)
            {
                clb.Items.Add(item);
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
            var fileNames = GetCheckedItems(deleteList);
            if (fileNames.Count == 0) return;

            SetControlEnabled(this, false);
            DTEHelper.RemoveFiles(dbConfig.Directory, fileNames);
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
            var templateConfig = ConfigHelper.GetTemplateConfig(dbConfig.TemplateId);
            if (templateConfig == null)
            {
                ShowError($"未找到 id 为 {dbConfig.TemplateId} 的实体模板");
                return;
            }

            var updateTableNames = GetCheckedItems(updateList);
            var newTableNames = GetCheckedItems(newList);
            updateTableNames.AddRange(newTableNames);
            GenerateFile(templateConfig.Content, updateTableNames);
        }

        private void GenerateFile(string template, List<string> updateTableNames)
        {
            try
            {
                SetControlEnabled(this, false);

                var fillTables = tables.Where(t => updateTableNames.Contains(t.Name)).ToList();
                db.FillColumnInfos(fillTables);

                var project = DTEHelper.GetSelectedProject();
                var path = Path.Combine(DTEHelper.GetProjectFullPath(project), dbConfig.Directory);
                DirectoryHelper.CreateDirectory(path);

                fillTables.AsParallel().ForAll(table =>
                {
                    table.FileName = $"{table.Name}.cs";
                    table.FileFullPath = Path.Combine(path, table.FileName);
                    var content = GenerateService.GetEntityContent(template, table);
                    GenerateService.SaveFile(table.FileFullPath, content);
                });

                foreach (var table in fillTables)
                {
                    project.ProjectItems.AddFromFile(table.FileFullPath);
                }

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

        private List<string> GetCheckedItems(CheckedListBox clb)
        {
            var list = new List<string>();
            for (int i = 0; i < clb.Items.Count; i++)
            {
                if (clb.GetItemChecked(i)) list.Add(clb.GetItemText(clb.Items[i]));
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
