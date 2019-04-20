using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using GrepLib;

namespace SimpleGrep
{
    public partial class FormFileList : Form
    {
        private enum Columns
        {
            No = 0,
            Dir,
            File,
            Ext,
            CreateDate,
            UpdateDate,
            Size_KB
        }

        private ResultContainer _rc = new ResultContainer();

        public FormFileList(ResultContainer rc)
        {
            InitializeComponent();

            _rc = rc;

            createColoumns();

            updateDataGridView();

            this.dataGridViewFileList.CellDoubleClick += OnCellDoubleClick;
        }

        private void OnCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }
            var dgv = sender as DataGridView;
            if(dgv == null)
            {
                return;
            }

            var dir = dgv[(int)Columns.Dir, e.RowIndex].Value.ToString();

            if(string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                return;
            }
            Process.Start(dir);
        }

        private void createColoumns()
        {
            // カラム数を指定
            this.dataGridViewFileList.ColumnCount = Enum.GetNames(typeof(Columns)).Length;

            // カラム名を指定
            this.dataGridViewFileList.Columns[0].HeaderText = "No";
            this.dataGridViewFileList.Columns[1].HeaderText = "Dir";
            this.dataGridViewFileList.Columns[2].HeaderText = "File";
            this.dataGridViewFileList.Columns[3].HeaderText = "Ext";
            this.dataGridViewFileList.Columns[4].HeaderText = "CreateDate";
            this.dataGridViewFileList.Columns[5].HeaderText = "UpdateDate";
            this.dataGridViewFileList.Columns[6].HeaderText = "Size(KB)";
        }

        private void updateDataGridView()
        {
            var index = 1;

            var path = string.Empty;
            var name = string.Empty;
            var ext = string.Empty;
            var createTime = DateTime.Now;
            var updateTime = DateTime.Now;
            var size = string.Empty;

            foreach(var result in _rc.Results)
            {
                if(result.File == null && result.Directory == null)
                {
                    continue;
                }

                if(result.File != null)
                {
                    path = result.File.DirectoryName;
                    name = result.File.Name;
                    ext = result.File.Extension;
                    createTime = result.File.CreationTime;
                    updateTime = result.File.LastWriteTime;

                    if(result.File.Length > 1024)
                    {
                        size = string.Format("{0:#,0}", result.File.Length / 1024);
                    }
                    else if(result.File.Length == 0)
                    {
                        size = "0";
                    }
                    else
                    {
                        size = "1";
                    }
                }
                else
                {
                    path = result.Directory.FullName;
                    name = result.Directory.Name;
                    ext = string.Empty;
                    createTime = result.Directory.CreationTime;
                    updateTime = result.Directory.LastWriteTime;
                    size = "0";
                }


                this.dataGridViewFileList.Rows.Add(
                    index++,
                    path,
                    name,
                    ext,
                    createTime,
                    updateTime,
                    size
                    );
            }
        }
    }
}
