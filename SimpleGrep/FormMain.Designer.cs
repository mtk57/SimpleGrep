namespace SimpleGrep
{
    partial class FormMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRefSearchDir = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxRegExp = new System.Windows.Forms.CheckBox();
            this.checkBoxIgnoreCase = new System.Windows.Forms.CheckBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.checkBoxIsExcludeBin = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxWordExcel = new System.Windows.Forms.CheckBox();
            this.buttonOpenOutputDIr = new System.Windows.Forms.Button();
            this.checkBoxWriteGrepInfo = new System.Windows.Forms.CheckBox();
            this.checkBoxSearchSubDir = new System.Windows.Forms.CheckBox();
            this.checkBoxExcludeHidden = new System.Windows.Forms.CheckBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.groupBoxOptions = new System.Windows.Forms.GroupBox();
            this.labelCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonRefOutputDir = new System.Windows.Forms.Button();
            this.checkBoxFileListMode = new System.Windows.Forms.CheckBox();
            this.comboBoxSearchDirPath = new System.Windows.Forms.ComboBox();
            this.comboBoxTargetFile = new System.Windows.Forms.ComboBox();
            this.comboBoxKeyword = new System.Windows.Forms.ComboBox();
            this.comboBoxOutputDirPath = new System.Windows.Forms.ComboBox();
            this.groupBoxOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "検索フォルダ";
            // 
            // buttonRefSearchDir
            // 
            this.buttonRefSearchDir.Location = new System.Drawing.Point(344, 24);
            this.buttonRefSearchDir.Name = "buttonRefSearchDir";
            this.buttonRefSearchDir.Size = new System.Drawing.Size(67, 19);
            this.buttonRefSearchDir.TabIndex = 20;
            this.buttonRefSearchDir.Text = "参照";
            this.buttonRefSearchDir.UseVisualStyleBackColor = true;
            this.buttonRefSearchDir.Click += new System.EventHandler(this.buttonRefSearchDir_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "キーワード";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "対象ファイル";
            // 
            // checkBoxRegExp
            // 
            this.checkBoxRegExp.AutoSize = true;
            this.checkBoxRegExp.Location = new System.Drawing.Point(15, 40);
            this.checkBoxRegExp.Name = "checkBoxRegExp";
            this.checkBoxRegExp.Size = new System.Drawing.Size(105, 16);
            this.checkBoxRegExp.TabIndex = 70;
            this.checkBoxRegExp.Text = "正規表現を使用";
            this.toolTip.SetToolTip(this.checkBoxRegExp, "単語単位の検索は「\\w」で囲む。(例.\\w単語\\w)");
            this.checkBoxRegExp.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreCase
            // 
            this.checkBoxIgnoreCase.AutoSize = true;
            this.checkBoxIgnoreCase.Location = new System.Drawing.Point(15, 62);
            this.checkBoxIgnoreCase.Name = "checkBoxIgnoreCase";
            this.checkBoxIgnoreCase.Size = new System.Drawing.Size(164, 16);
            this.checkBoxIgnoreCase.TabIndex = 90;
            this.checkBoxIgnoreCase.Text = "大文字・小文字を区別しない";
            this.checkBoxIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(176, 346);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(82, 27);
            this.buttonStart.TabIndex = 240;
            this.buttonStart.Text = "開始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // checkBoxIsExcludeBin
            // 
            this.checkBoxIsExcludeBin.AutoSize = true;
            this.checkBoxIsExcludeBin.Checked = true;
            this.checkBoxIsExcludeBin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIsExcludeBin.Location = new System.Drawing.Point(205, 40);
            this.checkBoxIsExcludeBin.Name = "checkBoxIsExcludeBin";
            this.checkBoxIsExcludeBin.Size = new System.Drawing.Size(127, 16);
            this.checkBoxIsExcludeBin.TabIndex = 80;
            this.checkBoxIsExcludeBin.Text = "バイナリファイルを除外";
            this.toolTip.SetToolTip(this.checkBoxIsExcludeBin, "拡張子がないファイルもバイナリーファイルと見なすので注意!");
            this.checkBoxIsExcludeBin.UseVisualStyleBackColor = true;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 500000;
            this.toolTip.InitialDelay = 1000;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100000;
            // 
            // checkBoxWordExcel
            // 
            this.checkBoxWordExcel.AutoSize = true;
            this.checkBoxWordExcel.Location = new System.Drawing.Point(15, 84);
            this.checkBoxWordExcel.Name = "checkBoxWordExcel";
            this.checkBoxWordExcel.Size = new System.Drawing.Size(146, 16);
            this.checkBoxWordExcel.TabIndex = 110;
            this.checkBoxWordExcel.Text = "Word,Excelの中身を検索";
            this.toolTip.SetToolTip(this.checkBoxWordExcel, "注意!!\r\nMS Officeのインストールが必要です\r\n検索性能が非常に低い場合があります（完了するまで気長にお待ち下さい）");
            this.checkBoxWordExcel.UseVisualStyleBackColor = true;
            // 
            // buttonOpenOutputDIr
            // 
            this.buttonOpenOutputDIr.Location = new System.Drawing.Point(344, 322);
            this.buttonOpenOutputDIr.Name = "buttonOpenOutputDIr";
            this.buttonOpenOutputDIr.Size = new System.Drawing.Size(67, 19);
            this.buttonOpenOutputDIr.TabIndex = 220;
            this.buttonOpenOutputDIr.Text = "開く";
            this.buttonOpenOutputDIr.UseVisualStyleBackColor = true;
            this.buttonOpenOutputDIr.Click += new System.EventHandler(this.buttonOpenOutputDir_Click);
            // 
            // checkBoxWriteGrepInfo
            // 
            this.checkBoxWriteGrepInfo.AutoSize = true;
            this.checkBoxWriteGrepInfo.Location = new System.Drawing.Point(205, 18);
            this.checkBoxWriteGrepInfo.Name = "checkBoxWriteGrepInfo";
            this.checkBoxWriteGrepInfo.Size = new System.Drawing.Size(144, 16);
            this.checkBoxWriteGrepInfo.TabIndex = 60;
            this.checkBoxWriteGrepInfo.Text = "GREP情報を結果に出力";
            this.checkBoxWriteGrepInfo.UseVisualStyleBackColor = true;
            // 
            // checkBoxSearchSubDir
            // 
            this.checkBoxSearchSubDir.AutoSize = true;
            this.checkBoxSearchSubDir.Checked = true;
            this.checkBoxSearchSubDir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSearchSubDir.Location = new System.Drawing.Point(15, 18);
            this.checkBoxSearchSubDir.Name = "checkBoxSearchSubDir";
            this.checkBoxSearchSubDir.Size = new System.Drawing.Size(111, 16);
            this.checkBoxSearchSubDir.TabIndex = 50;
            this.checkBoxSearchSubDir.Text = "サブフォルダも検索";
            this.checkBoxSearchSubDir.UseVisualStyleBackColor = true;
            // 
            // checkBoxExcludeHidden
            // 
            this.checkBoxExcludeHidden.AutoSize = true;
            this.checkBoxExcludeHidden.Checked = true;
            this.checkBoxExcludeHidden.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExcludeHidden.Location = new System.Drawing.Point(204, 62);
            this.checkBoxExcludeHidden.Name = "checkBoxExcludeHidden";
            this.checkBoxExcludeHidden.Size = new System.Drawing.Size(153, 16);
            this.checkBoxExcludeHidden.TabIndex = 100;
            this.checkBoxExcludeHidden.Text = "隠しフォルダ/ファイルを除外";
            this.checkBoxExcludeHidden.UseVisualStyleBackColor = true;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(264, 361);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(27, 12);
            this.labelTime.TabIndex = 11;
            this.labelTime.Text = "time";
            // 
            // groupBoxOptions
            // 
            this.groupBoxOptions.Controls.Add(this.checkBoxWordExcel);
            this.groupBoxOptions.Controls.Add(this.checkBoxExcludeHidden);
            this.groupBoxOptions.Controls.Add(this.checkBoxSearchSubDir);
            this.groupBoxOptions.Controls.Add(this.checkBoxWriteGrepInfo);
            this.groupBoxOptions.Controls.Add(this.checkBoxRegExp);
            this.groupBoxOptions.Controls.Add(this.checkBoxIgnoreCase);
            this.groupBoxOptions.Controls.Add(this.checkBoxIsExcludeBin);
            this.groupBoxOptions.Location = new System.Drawing.Point(14, 149);
            this.groupBoxOptions.Name = "groupBoxOptions";
            this.groupBoxOptions.Size = new System.Drawing.Size(397, 117);
            this.groupBoxOptions.TabIndex = 49;
            this.groupBoxOptions.TabStop = false;
            this.groupBoxOptions.Text = "オプション";
            // 
            // labelCount
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(264, 346);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(33, 12);
            this.labelCount.TabIndex = 16;
            this.labelCount.Text = "count";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 282);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 12);
            this.label4.TabIndex = 17;
            this.label4.Text = "結果ファイルの出力先フォルダ";
            // 
            // buttonRefOutputDir
            // 
            this.buttonRefOutputDir.Location = new System.Drawing.Point(344, 297);
            this.buttonRefOutputDir.Name = "buttonRefOutputDir";
            this.buttonRefOutputDir.Size = new System.Drawing.Size(67, 19);
            this.buttonRefOutputDir.TabIndex = 210;
            this.buttonRefOutputDir.Text = "参照";
            this.buttonRefOutputDir.UseVisualStyleBackColor = true;
            this.buttonRefOutputDir.Click += new System.EventHandler(this.buttonRefOutputDir_Click);
            // 
            // checkBoxFileListMode
            // 
            this.checkBoxFileListMode.AutoSize = true;
            this.checkBoxFileListMode.Location = new System.Drawing.Point(14, 346);
            this.checkBoxFileListMode.Name = "checkBoxFileListMode";
            this.checkBoxFileListMode.Size = new System.Drawing.Size(110, 16);
            this.checkBoxFileListMode.TabIndex = 230;
            this.checkBoxFileListMode.Text = "ファイル一覧モード";
            this.checkBoxFileListMode.UseVisualStyleBackColor = true;
            this.checkBoxFileListMode.CheckedChanged += new System.EventHandler(this.checkBoxFileListMode_CheckedChanged);
            // 
            // comboBoxSearchDirPath
            // 
            this.comboBoxSearchDirPath.AllowDrop = true;
            this.comboBoxSearchDirPath.FormattingEnabled = true;
            this.comboBoxSearchDirPath.Location = new System.Drawing.Point(14, 23);
            this.comboBoxSearchDirPath.Name = "comboBoxSearchDirPath";
            this.comboBoxSearchDirPath.Size = new System.Drawing.Size(324, 20);
            this.comboBoxSearchDirPath.TabIndex = 10;
            this.comboBoxSearchDirPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.comboBoxSearchDirPath_DragDrop);
            this.comboBoxSearchDirPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.comboBoxSearchDirPath_DragEnter);
            // 
            // comboBoxTargetFile
            // 
            this.comboBoxTargetFile.FormattingEnabled = true;
            this.comboBoxTargetFile.Location = new System.Drawing.Point(14, 71);
            this.comboBoxTargetFile.Name = "comboBoxTargetFile";
            this.comboBoxTargetFile.Size = new System.Drawing.Size(397, 20);
            this.comboBoxTargetFile.TabIndex = 30;
            // 
            // comboBoxKeyword
            // 
            this.comboBoxKeyword.FormattingEnabled = true;
            this.comboBoxKeyword.Location = new System.Drawing.Point(14, 123);
            this.comboBoxKeyword.Name = "comboBoxKeyword";
            this.comboBoxKeyword.Size = new System.Drawing.Size(397, 20);
            this.comboBoxKeyword.TabIndex = 40;
            // 
            // comboBoxOutputDirPath
            // 
            this.comboBoxOutputDirPath.AllowDrop = true;
            this.comboBoxOutputDirPath.FormattingEnabled = true;
            this.comboBoxOutputDirPath.Location = new System.Drawing.Point(12, 297);
            this.comboBoxOutputDirPath.Name = "comboBoxOutputDirPath";
            this.comboBoxOutputDirPath.Size = new System.Drawing.Size(326, 20);
            this.comboBoxOutputDirPath.TabIndex = 200;
            this.comboBoxOutputDirPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.comboBoxOutputDirPath_DragDrop);
            this.comboBoxOutputDirPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.comboBoxOutputDirPath_DragEnter);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 383);
            this.Controls.Add(this.comboBoxOutputDirPath);
            this.Controls.Add(this.comboBoxKeyword);
            this.Controls.Add(this.comboBoxTargetFile);
            this.Controls.Add(this.comboBoxSearchDirPath);
            this.Controls.Add(this.checkBoxFileListMode);
            this.Controls.Add(this.buttonRefOutputDir);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelCount);
            this.Controls.Add(this.groupBoxOptions);
            this.Controls.Add(this.buttonOpenOutputDIr);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonRefSearchDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "Simple GREP";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBoxOptions.ResumeLayout(false);
            this.groupBoxOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonRefSearchDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxRegExp;
        private System.Windows.Forms.CheckBox checkBoxIgnoreCase;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.CheckBox checkBoxIsExcludeBin;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Button buttonOpenOutputDIr;
        private System.Windows.Forms.CheckBox checkBoxWriteGrepInfo;
        private System.Windows.Forms.CheckBox checkBoxSearchSubDir;
        private System.Windows.Forms.GroupBox groupBoxOptions;
        private System.Windows.Forms.CheckBox checkBoxExcludeHidden;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonRefOutputDir;
        private System.Windows.Forms.CheckBox checkBoxFileListMode;
        private System.Windows.Forms.ComboBox comboBoxSearchDirPath;
        private System.Windows.Forms.ComboBox comboBoxTargetFile;
        private System.Windows.Forms.ComboBox comboBoxKeyword;
        private System.Windows.Forms.ComboBox comboBoxOutputDirPath;
        private System.Windows.Forms.CheckBox checkBoxWordExcel;
    }
}

