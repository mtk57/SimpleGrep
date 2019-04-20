using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GrepLib;

namespace SimpleGrep
{
    public partial class FormMain : Form
    {
        /*
        MUST
          ・高速化
              初回検索は遅いみたい（キャッシュが効かないため?）⇒STARTボタン押すまでの間に裏で検索を行う?(やりすぎ?)

              とりあえずジャストアイデアだけ残しておく（本当に速くなるかは知らん）
               ・しきい値までファイルをメモリに読み込みまくるスレッドA
               ・スレッドAが溜め込んだメモリを検索し結果をそれぞれが持つリストに書き込むスレッドB (最大n個)
                 ⇒スレッドBはパラレルで実行されるので、各リストは順序がバラバラ（最後にソートする）
               ・今までのシングルスレッドでの読み込み・検索・結果書き込みを行うスレッドC
               ・スレッドAとスレッドCはパラレルで実行されるので、早く終わったほうが採用される
                 ⇒ファイル数が少ない場合はスレッドC、多い場合はスレッドAが有利になるように。
               
               ・再帰処理をやめることは可能か？（多分効果薄いと思う。最内ループのsearchKeywordだけをチューニングすべき）
               ・dynamicをやめる
               ・newをやめる
               ・
        WANT
          ・Word,Excelの中身を検索
            xlsx:COM  ⇒メッチャ遅いのでサブスレッド化すること!
                 OSS
                  ⇒NanoXLSX:不採用
                  ⇒POI:不採用
            xls :xlsxと同じ
            doc :xlsxと同じ
            docx:xlsxと同じ
        */
        private const string VERSION = "1.4d";

        private const int MAX_HISTORY = 10;

        private const string DEBUG_DIR = @"C:\_tmp\GREP_TEST";
        private const string DEBUG_FILES = @"*.cs;*.js;!*.txt";
        private const string DEBUG_KEYWORD = @"var";
        private const string DEBUG_OUTDIR = @"C:\_tmp";

        private const string FORMAT_GREP_INFO =
@"■KEYWORD         :{0}
■TARGET FILE     :{1}
■TARGET DIR      :{2}

■SEARCH SUB DIR  :{3}
■REG EXP         :{4}
■IGNORE CASE     :{5}
■EXCLUDE BIN FILE:{6}
■EXCLUDE HIDDEN  :{7}
■WORD EXCEL      :{8}
■COUNT           :{9}
----------------------
";

        private FormProcessing _fp = null;
        private Grep _grep = null;
        private Stopwatch _sw = new Stopwatch();

        private List<ComboBox> _combos = new List<ComboBox>();

        public FormMain()
        {
            InitializeComponent();

            _fp = new FormProcessing(new DoWorkEventHandler(doWork));

            _combos.Add(this.comboBoxSearchDirPath);
            _combos.Add(this.comboBoxTargetFile);
            _combos.Add(this.comboBoxKeyword);
            _combos.Add(this.comboBoxOutputDirPath);

            foreach(var cmb in _combos)
            {
                cmb.MaxDropDownItems = MAX_HISTORY;
                cmb.IntegralHeight = false;
            }

            this.Text = string.Format("SimpleGrep  ver {0}", VERSION);

#if DEBUG
            this.comboBoxSearchDirPath.Text = DEBUG_DIR;
            this.comboBoxTargetFile.Text = DEBUG_FILES;
            this.comboBoxKeyword.Text = DEBUG_KEYWORD;
            this.comboBoxOutputDirPath.Text = DEBUG_OUTDIR;

            //save();
#endif
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = false;
            this.labelCount.Text = string.Empty;
            this.labelTime.Text = string.Empty;

            try
            {
                if(!prepare())
                {
                    Utils.ShowMessageBoxAndWriteLog("prepare failed");
                    return;
                }

                if(this.checkBoxWordExcel.Checked && !this.checkBoxFileListMode.Checked)
                {
                    //MsOfficeHelper.CreateMsOfficeApp();
                }

                grepMain();
            }
            catch(Exception ex)
            {
                Utils.ShowMessageBoxAndWriteLogForException(ex);
            }
            finally
            {
                this.buttonStart.Enabled = true;

                if(this.checkBoxWordExcel.Checked && !this.checkBoxFileListMode.Checked)
                {
                    //MsOfficeHelper.ReleaseMsOfficeApp();
                }
            }
        }

        private bool prepare()
        {
            // チェックは最低限のみ
            if(this.checkBoxFileListMode.Checked == false)
            {
                if(string.IsNullOrEmpty(this.comboBoxSearchDirPath.Text)
                    || string.IsNullOrEmpty(this.comboBoxTargetFile.Text)
                    || string.IsNullOrEmpty(this.comboBoxKeyword.Text)
                    || string.IsNullOrEmpty(this.comboBoxOutputDirPath.Text)
                    )
                {
                    return false;
                }
            }
            else
            {
                if(string.IsNullOrEmpty(this.comboBoxSearchDirPath.Text)
                    || string.IsNullOrEmpty(this.comboBoxTargetFile.Text)
                    )
                {
                    return false;
                }
            }

            return true;
        }

        private void grepMain()
        {
            var dir = new DirectoryInfo(this.comboBoxSearchDirPath.Text);
            if(!dir.Exists)
            {
                Utils.ShowMessageBoxAndWriteLog("Search directory is not exist!");
                return;
            }

            DirectoryInfo outDir = null;
            if (!this.checkBoxFileListMode.Checked)
            {
                // GREPモード
                outDir = new DirectoryInfo(this.comboBoxOutputDirPath.Text);
                if (!outDir.Exists && !this.checkBoxFileListMode.Checked)
                {
                    Utils.ShowMessageBoxAndWriteLog("Output directory is not exist!");
                    return;
                }
            }

            var files = new List<FilePattern>();
            foreach(var ptn in this.comboBoxTargetFile.Text.Split(';').ToList())
            {
                files.Add(new FilePattern(ptn));
            }

            var keyword = this.comboBoxKeyword.Text;

            _sw.Reset();
            _sw.Start();

            var results = runGrep(new Grep(dir, files, keyword, new Option(
                            this.checkBoxRegExp.Checked,
                            this.checkBoxIgnoreCase.Checked,
                            this.checkBoxIsExcludeBin.Checked,
                            this.checkBoxSearchSubDir.Checked,
                            this.checkBoxExcludeHidden.Checked,
                            this.checkBoxFileListMode.Checked,
                            this.checkBoxWordExcel.Checked
                            )));

            _sw.Stop();
            this.labelTime.Text = _sw.Elapsed.ToString();
            this.labelCount.Text = results.NumberOfSuccesses.ToString();

            updateComboBox();


            var grepInfo = string.Empty;

            if(this.checkBoxWriteGrepInfo.Checked && this.checkBoxFileListMode.Checked == false)
            {
                grepInfo = string.Format(FORMAT_GREP_INFO,
                                this.comboBoxKeyword.Text,
                                this.comboBoxTargetFile.Text,
                                this.comboBoxSearchDirPath.Text,

                                this.checkBoxSearchSubDir.Checked,
                                this.checkBoxRegExp.Checked,
                                this.checkBoxIgnoreCase.Checked,
                                this.checkBoxIsExcludeBin.Checked,
                                this.checkBoxExcludeHidden.Checked,
                                this.checkBoxWordExcel.Checked,
                                results.NumberOfSuccesses
                                );
            }

            if(this.checkBoxWriteGrepInfo.Checked || results.NumberOfSuccesses > 0)
            {
                if(!this.checkBoxFileListMode.Checked)
                {
                    // GREPモード
                    var resultFilePath = Utils.WriteTextFile(
                                                outDir,
                                                grepInfo + results.ToString(),
                                                ".grep");

                    Process.Start(resultFilePath);
                }
                else
                {
                    // ファイル一覧モード
                    if(results.NumberOfSuccesses > 0)
                    {
                        var ffl = new FormFileList(results);
                        ffl.Text = string.Format("{0}  Count={1}", this.comboBoxTargetFile.Text, results.NumberOfSuccesses);
                        ffl.Show();
                    }
                }
            }
        }

        private ResultContainer runGrep(Grep grep)
        {
            var results = new ResultContainer();

            _grep = grep;

            _fp.Start();
            var result = _fp.ShowDialog(this);

            if(result == DialogResult.Cancel)
            {
            }
            else if(result == DialogResult.Abort)
            {
                Exception ex = _fp.Error;
                Utils.ShowMessageBoxAndWriteLogForException(ex);
            }
            else if(result == DialogResult.OK)
            {
                results = (ResultContainer)_fp.Result;
            }

            return results;
        }

        private void doWork(object sender, DoWorkEventArgs e)
        {
            e.Result = _grep.Execute((BackgroundWorker)sender, e);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                if(!File.Exists(SaveData.FileName))
                {
                    return;
                }

                var save = Utils.XmlToObject<SaveData>(SaveData.FileName, typeof(SaveData));
                if(save == null)
                {
                    return;
                }

                setComboBoxData(this.comboBoxSearchDirPath, save.SearchDirectoryPath);
                setComboBoxData(this.comboBoxTargetFile, save.TargetFile);
                setComboBoxData(this.comboBoxKeyword, save.Keyword);
                setComboBoxData(this.comboBoxOutputDirPath, save.OutputDirectoryPath);

                this.checkBoxRegExp.Checked = save.RegExp;
                this.checkBoxIgnoreCase.Checked = save.IgnoreCase;
                this.checkBoxIsExcludeBin.Checked = save.ExcludeBinaryFile;
                this.checkBoxWriteGrepInfo.Checked = save.WriteGrepInfo;
                this.checkBoxSearchSubDir.Checked = save.SearchSubDirectories;
                this.checkBoxWordExcel.Checked = save.WordExcel;

                this.checkBoxFileListMode.Checked = save.FileListMode;
            }
            catch(Exception ex)
            {
                Utils.ShowMessageBoxAndWriteLogForException(ex);
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                updateComboBox();

                save();
            }
            catch(Exception ex)
            {
                Utils.ShowMessageBoxAndWriteLogForException(ex);
            }
        }

        private void save()
        {
            var save = new SaveData(
                        getComboBoxData(this.comboBoxSearchDirPath),
                        getComboBoxData(this.comboBoxTargetFile),
                        getComboBoxData(this.comboBoxKeyword),
                        getComboBoxData(this.comboBoxOutputDirPath),

                        this.checkBoxRegExp.Checked,
                        this.checkBoxIgnoreCase.Checked,
                        this.checkBoxIsExcludeBin.Checked,
                        this.checkBoxWriteGrepInfo.Checked,
                        this.checkBoxSearchSubDir.Checked,
                        this.checkBoxExcludeHidden.Checked,
                        this.checkBoxFileListMode.Checked,
                        this.checkBoxWordExcel.Checked
                        );

            Utils.ObjectToXml<SaveData>(SaveData.FileName, save);
        }

        private void comboBoxSearchDirPath_DragDrop(object sender, DragEventArgs e)
        {
            this.comboBoxSearchDirPath.Text = getDropData(e);
        }

        private void comboBoxSearchDirPath_DragEnter(object sender, DragEventArgs e)
        {
            startDragDrop(e);
        }

        private void comboBoxOutputDirPath_DragDrop(object sender, DragEventArgs e)
        {
            this.comboBoxOutputDirPath.Text = getDropData(e);
        }

        private void comboBoxOutputDirPath_DragEnter(object sender, DragEventArgs e)
        {
            startDragDrop(e);
        }

        private void startDragDrop(DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private string getDropData(DragEventArgs e)
        {
            var data = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if(data == null || data.Length == 0 || data.Length > 1)
            {
                return string.Empty;
            }

            return data[0];
        }

        private void buttonRefSearchDir_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            fbd.Description = "Select search directory";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.SelectedPath = this.comboBoxSearchDirPath.Text;
            fbd.ShowNewFolderButton = true;
            if(fbd.ShowDialog(this) == DialogResult.OK)
            {
                this.comboBoxSearchDirPath.Text = fbd.SelectedPath;
            }
        }

        private void buttonRefOutputDir_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            fbd.Description = "Select output directory";
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.SelectedPath = this.comboBoxOutputDirPath.Text;
            fbd.ShowNewFolderButton = true;
            if(fbd.ShowDialog(this) == DialogResult.OK)
            {
                this.comboBoxOutputDirPath.Text = fbd.SelectedPath;
            }
        }

        private void buttonOpenOutputDir_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.comboBoxOutputDirPath.Text) ||
                !Directory.Exists(this.comboBoxOutputDirPath.Text))
            {
                return;
            }
            Process.Start(this.comboBoxOutputDirPath.Text);
        }

        private void checkBoxFileListMode_CheckedChanged(object sender, EventArgs e)
        {
            var status = !this.checkBoxFileListMode.Checked;
            this.comboBoxKeyword.Enabled = status;
            this.groupBoxOptions.Enabled = status;
            this.comboBoxOutputDirPath.Enabled = status;
            this.buttonRefOutputDir.Enabled = status;
            this.buttonOpenOutputDIr.Enabled = status;
        }

        private void setComboBoxData(ComboBox cmb, List<string> data)
        {
            cmb.Text = string.Empty;
            cmb.Items.Clear();

            if(data == null || data.Count == 0)
            {
                return;
            }

            cmb.Text = data[0];

            for(var i=0; i< data.Count; i++)
            {
                if(i > cmb.MaxDropDownItems)
                {
                    break;
                }
                cmb.Items.Add(data[i]);
            }
        }

        private List<string> getComboBoxData(ComboBox cmb)
        {
            var ret = new List<string>(cmb.MaxDropDownItems);

            if(cmb.Items.Count == 0)
            {
                ret.Add(cmb.Text);
            }

            foreach(var item in cmb.Items)
            {
                ret.Add(item.ToString());
            }
            return ret;
        }

        private void updateComboBox()
        {
            foreach(var cmb in _combos)
            {
                //if(cmb.Items.Contains(cmb.Text) || string.IsNullOrEmpty(cmb.Text))
                //{
                //    // コンボボックスにすでに存在するので無視
                //    continue;
                //}

                if(cmb.Items.Count == cmb.MaxDropDownItems)
                {
                    // 末尾を削除
                    cmb.Items.RemoveAt(cmb.Items.Count - 1);
                }
                cmb.Items.Insert(0, cmb.Text);
            }
        }
    }
}
