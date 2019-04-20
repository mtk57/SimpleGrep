using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GrepLib
{
    public partial class FormProcessing : Form
    {
        private object _result = null;
        public object Result
        {
            get
            {
                return _result;
            }
        }

        private Exception _error = null;
        public Exception Error
        {
            get
            {
                return _error;
            }
        }

        public BackgroundWorker BackgroundWorker
        {
            get
            {
                return this.backgroundWorker;
            }
        }

        private FormProcessing()
        {
            // 封印
        }

        public FormProcessing(DoWorkEventHandler doWork)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ShowInTaskbar = false;
            this.labelCount.Text = string.Empty;
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;

            this.backgroundWorker.DoWork += doWork;
            this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        public void Start()
        {
            this.labelCount.Text = string.Empty;
            this.buttonCancel.Enabled = true;
            this.backgroundWorker.RunWorkerAsync();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.buttonCancel.Enabled = false;
            backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.labelCount.Text = e.ProgressPercentage.ToString();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                Utils.ShowMessageBoxAndWriteLog(e.Error.Message);
                this._error = e.Error;
                this.DialogResult = DialogResult.Abort;
            }
            else if(e.Cancelled)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this._result = e.Result;
                this.DialogResult = DialogResult.OK;
            }

            this.Close();
        }
    }
}
