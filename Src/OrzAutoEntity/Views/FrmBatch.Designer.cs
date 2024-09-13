
namespace OrzAutoEntity.Views
{
    partial class FrmBatch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbDatabase = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCancelUpdate = new System.Windows.Forms.Button();
            this.btnAllUpdate = new System.Windows.Forms.Button();
            this.updateList = new System.Windows.Forms.CheckedListBox();
            this.txtUpdateFilter = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCancelNew = new System.Windows.Forms.Button();
            this.btnAllNew = new System.Windows.Forms.Button();
            this.newList = new System.Windows.Forms.CheckedListBox();
            this.txtNewFilter = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnCancelDelete = new System.Windows.Forms.Button();
            this.btnAllDelete = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.deleteList = new System.Windows.Forms.CheckedListBox();
            this.txtDeleteFilter = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbConnString = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbDatabase);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(920, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择数据源";
            // 
            // cbDatabase
            // 
            this.cbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDatabase.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbDatabase.FormattingEnabled = true;
            this.cbDatabase.Location = new System.Drawing.Point(6, 20);
            this.cbDatabase.Name = "cbDatabase";
            this.cbDatabase.Size = new System.Drawing.Size(291, 24);
            this.cbDatabase.TabIndex = 0;
            this.cbDatabase.SelectedIndexChanged += new System.EventHandler(this.cbDatabase_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnCancelUpdate);
            this.groupBox2.Controls.Add(this.btnAllUpdate);
            this.groupBox2.Controls.Add(this.updateList);
            this.groupBox2.Controls.Add(this.txtUpdateFilter);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 514);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "已生成 Model";
            // 
            // btnCancelUpdate
            // 
            this.btnCancelUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelUpdate.Location = new System.Drawing.Point(219, 485);
            this.btnCancelUpdate.Name = "btnCancelUpdate";
            this.btnCancelUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnCancelUpdate.TabIndex = 3;
            this.btnCancelUpdate.Text = "全取消";
            this.btnCancelUpdate.UseVisualStyleBackColor = true;
            this.btnCancelUpdate.Click += new System.EventHandler(this.btnCancelUpdate_Click);
            // 
            // btnAllUpdate
            // 
            this.btnAllUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAllUpdate.Location = new System.Drawing.Point(138, 485);
            this.btnAllUpdate.Name = "btnAllUpdate";
            this.btnAllUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnAllUpdate.TabIndex = 2;
            this.btnAllUpdate.Text = "全选";
            this.btnAllUpdate.UseVisualStyleBackColor = true;
            this.btnAllUpdate.Click += new System.EventHandler(this.btnAllUpdate_Click);
            // 
            // updateList
            // 
            this.updateList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.updateList.CheckOnClick = true;
            this.updateList.FormattingEnabled = true;
            this.updateList.Location = new System.Drawing.Point(6, 47);
            this.updateList.Name = "updateList";
            this.updateList.Size = new System.Drawing.Size(288, 420);
            this.updateList.TabIndex = 1;
            // 
            // txtUpdateFilter
            // 
            this.txtUpdateFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUpdateFilter.Location = new System.Drawing.Point(6, 20);
            this.txtUpdateFilter.Name = "txtUpdateFilter";
            this.txtUpdateFilter.Size = new System.Drawing.Size(288, 21);
            this.txtUpdateFilter.TabIndex = 0;
            this.txtUpdateFilter.TextChanged += new System.EventHandler(this.txtUpdateFilter_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnCancelNew);
            this.groupBox3.Controls.Add(this.btnAllNew);
            this.groupBox3.Controls.Add(this.newList);
            this.groupBox3.Controls.Add(this.txtNewFilter);
            this.groupBox3.Location = new System.Drawing.Point(309, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(300, 514);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "未生成 Model";
            // 
            // btnCancelNew
            // 
            this.btnCancelNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelNew.Location = new System.Drawing.Point(219, 485);
            this.btnCancelNew.Name = "btnCancelNew";
            this.btnCancelNew.Size = new System.Drawing.Size(75, 23);
            this.btnCancelNew.TabIndex = 3;
            this.btnCancelNew.Text = "全取消";
            this.btnCancelNew.UseVisualStyleBackColor = true;
            this.btnCancelNew.Click += new System.EventHandler(this.btnCancelNew_Click);
            // 
            // btnAllNew
            // 
            this.btnAllNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAllNew.Location = new System.Drawing.Point(138, 485);
            this.btnAllNew.Name = "btnAllNew";
            this.btnAllNew.Size = new System.Drawing.Size(75, 23);
            this.btnAllNew.TabIndex = 2;
            this.btnAllNew.Text = "全选";
            this.btnAllNew.UseVisualStyleBackColor = true;
            this.btnAllNew.Click += new System.EventHandler(this.btnAllNew_Click);
            // 
            // newList
            // 
            this.newList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.newList.CheckOnClick = true;
            this.newList.FormattingEnabled = true;
            this.newList.Location = new System.Drawing.Point(6, 47);
            this.newList.Name = "newList";
            this.newList.Size = new System.Drawing.Size(288, 420);
            this.newList.TabIndex = 1;
            // 
            // txtNewFilter
            // 
            this.txtNewFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewFilter.Location = new System.Drawing.Point(6, 20);
            this.txtNewFilter.Name = "txtNewFilter";
            this.txtNewFilter.Size = new System.Drawing.Size(288, 21);
            this.txtNewFilter.TabIndex = 0;
            this.txtNewFilter.TextChanged += new System.EventHandler(this.txtNewFilter_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btnCancelDelete);
            this.groupBox4.Controls.Add(this.btnAllDelete);
            this.groupBox4.Controls.Add(this.btnDelete);
            this.groupBox4.Controls.Add(this.deleteList);
            this.groupBox4.Controls.Add(this.txtDeleteFilter);
            this.groupBox4.Location = new System.Drawing.Point(615, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(302, 514);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Model 在库中找不到对应表";
            // 
            // btnCancelDelete
            // 
            this.btnCancelDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelDelete.Location = new System.Drawing.Point(140, 485);
            this.btnCancelDelete.Name = "btnCancelDelete";
            this.btnCancelDelete.Size = new System.Drawing.Size(75, 23);
            this.btnCancelDelete.TabIndex = 3;
            this.btnCancelDelete.Text = "全取消";
            this.btnCancelDelete.UseVisualStyleBackColor = true;
            this.btnCancelDelete.Click += new System.EventHandler(this.btnCancelDelete_Click);
            // 
            // btnAllDelete
            // 
            this.btnAllDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAllDelete.Location = new System.Drawing.Point(59, 485);
            this.btnAllDelete.Name = "btnAllDelete";
            this.btnAllDelete.Size = new System.Drawing.Size(75, 23);
            this.btnAllDelete.TabIndex = 2;
            this.btnAllDelete.Text = "全选";
            this.btnAllDelete.UseVisualStyleBackColor = true;
            this.btnAllDelete.Click += new System.EventHandler(this.btnAllDelete_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(221, 485);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // deleteList
            // 
            this.deleteList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteList.CheckOnClick = true;
            this.deleteList.FormattingEnabled = true;
            this.deleteList.Location = new System.Drawing.Point(6, 47);
            this.deleteList.Name = "deleteList";
            this.deleteList.Size = new System.Drawing.Size(290, 420);
            this.deleteList.TabIndex = 1;
            // 
            // txtDeleteFilter
            // 
            this.txtDeleteFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeleteFilter.Location = new System.Drawing.Point(6, 20);
            this.txtDeleteFilter.Name = "txtDeleteFilter";
            this.txtDeleteFilter.Size = new System.Drawing.Size(290, 21);
            this.txtDeleteFilter.TabIndex = 0;
            this.txtDeleteFilter.TextChanged += new System.EventHandler(this.txtDeleteFilter_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lbConnString});
            this.statusStrip1.Location = new System.Drawing.Point(0, 659);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(944, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "当前连接：";
            // 
            // lbConnString
            // 
            this.lbConnString.Name = "lbConnString";
            this.lbConnString.Size = new System.Drawing.Size(861, 17);
            this.lbConnString.Spring = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(676, 614);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(150, 30);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "添加/刷新选中项";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(832, 614);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 77);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(920, 520);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // FrmBatch
            // 
            this.AcceptButton = this.btnRefresh;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(944, 681);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(800, 39);
            this.Name = "FrmBatch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmBatch";
            this.Load += new System.EventHandler(this.FrmBatch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbDatabase;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCancelUpdate;
        private System.Windows.Forms.Button btnAllUpdate;
        private System.Windows.Forms.CheckedListBox updateList;
        private System.Windows.Forms.TextBox txtUpdateFilter;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCancelNew;
        private System.Windows.Forms.Button btnAllNew;
        private System.Windows.Forms.CheckedListBox newList;
        private System.Windows.Forms.TextBox txtNewFilter;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnCancelDelete;
        private System.Windows.Forms.Button btnAllDelete;
        private System.Windows.Forms.CheckedListBox deleteList;
        private System.Windows.Forms.TextBox txtDeleteFilter;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lbConnString;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}