using System;
using System.Reflection;
using System.Windows.Forms;
using OrzAutoEntity.Helpers;

namespace OrzAutoEntity.Views
{
    public partial class FrmBatch : Form
    {
        public FrmBatch()
        {
            InitializeComponent();
        }

        private void FrmBatch_Load(object sender, EventArgs e)
        {
            Text = $"实体类生成工具 {GetVersion()}";
            cbDatabase.DisplayMember = "Name";
            ConfigHelper.Databases.ForEach(t => cbDatabase.Items.Add(t));
        }

        private void cbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        #region 过滤
        private void txtUpdateFilter_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNewFilter_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDeleteFilter_TextChanged(object sender, EventArgs e)
        {

        }

        private void FilterList()
        {

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

        private void SetItemChecked(CheckedListBox list, bool isChecked)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                list.SetItemChecked(i, isChecked);
            }
        }
        #endregion

        private void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"V{version.Major}.{version.Minor}";
        }
    }
}
