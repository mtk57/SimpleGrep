using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrepLib
{
    public class Grep
    {
        private const string MSG_SEARCH_FAILED = "Search failed. ({0})";

        /// <summary>
        /// 検索するディレクトリー
        /// </summary>
        public DirectoryInfo Dir { get; }

        /// <summary>
        /// 検索するファイルパターン
        /// </summary>
        public FilePatternContainer Patterns { get; }

        /// <summary>
        /// 検索キーワード
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// 検索オプション
        /// </summary>
        public Option Option { get; }

        public BackgroundWorker Worker { get; set; }
        public DoWorkEventArgs WorkerEventArgs { get; set; }

        private Regex _regExForKeyword = null;
        private Stopwatch _sw = new Stopwatch();
        private Stopwatch _sw2 = new Stopwatch();

        private Grep()
        {
            // 封印
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="dir">検索するディレクトリー</param>
        /// <param name="patterns">検索するファイルパターン</param>
        /// <param name="keyword">検索キーワード</param>
        /// <param name="option">検索オプション</param>
        public Grep(DirectoryInfo dir, List<FilePattern> patterns, string keyword, Option option)
        {
            this.Dir = dir;
            this.Patterns = new FilePatternContainer(patterns);
            this.Keyword = keyword;
            this.Option = option;

            // 正規表現を使用?
            if(this.Option.RegExp)
            {
                if(this.Option.IgnoreCase)
                {
                    // 大文字小文字を区別しない
                    _regExForKeyword = new Regex(this.Keyword, RegexOptions.IgnoreCase);
                }
                else
                {
                    // 大文字小文字を区別
                    _regExForKeyword = new Regex(this.Keyword);
                }
            }
        }

        /// <summary>
        /// 検索実行
        /// </summary>
        /// <param name="bw">BackgroundWorker</param>
        /// <param name="e">DoWorkEventArgs</param>
        /// <returns>検索結果</returns>
        public ResultContainer Execute(BackgroundWorker bw, DoWorkEventArgs e)
        {
            this.Worker = bw;
            this.WorkerEventArgs = e;

            var results = new ResultContainer();

            // 指定ディレクトリー直下を検索
            searchDirectory(this.Dir, results);

            return results;
        }

        /// <summary>
        /// ディレクトリーのファイル内からキーワードを検索して結果を返す
        /// サブディレクトリーも対象とする
        /// </summary>
        /// <param name="dir">ディレクトリー</param>
        /// <param name="results">検索結果</param>
        private void searchDirectory(DirectoryInfo dir, ResultContainer results)
        {
            try
            {
                // ディレクトリー直下のファイルを検索
                foreach(var file in dir.EnumerateFiles())
                {
                    if(this.Worker.CancellationPending)
                    {
                        //this.WorkerEventArgs.Cancel = true;
                        return;
                    }

                    if(file.Length == 0)
                    {
                        // 空ファイルは無視
                        continue;
                    }

                    if(this.Option.FileListMode == false)
                    {
                        // GREPモード

                        if(this.Option.ExcludeHidden)
                        {
                            if((file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                // 隠しファイルは無視
                                continue;
                            }
                        }

                        if(Patterns.IsExcludeFile(file))
                        {
                            // !が指定された拡張子は無視
                            continue;
                        }
                        if(!Patterns.IsTargetFile(file))
                        {
                            // 検索対象の拡張子以外は無視
                            continue;
                        }

                        //_sw.Start();

                        // ファイル内をキーワード検索
                        searchKeyword(file, results);

                        //_sw.Stop();

                        //Logger.Write(string.Format("\t{0}\t'{1}\t{2}\t{3}", _sw.ElapsedMilliseconds, _sw.Elapsed, resultType, file.FullName));
                        //_sw.Reset();
                    }
                    else
                    {
                        if(!Patterns.IsTargetFile(file, true))
                        {
                            // 検索対象の拡張子以外は無視
                            continue;
                        }

                        // ファイル一覧モード
                        collectFile(file, results);
                    }
                }

                if(!this.Option.SearchSubDir && this.Option.FileListMode == false)
                {
                    return;
                }

                // サブディレクトリーを検索
                foreach(var subDir in dir.EnumerateDirectories())
                {
                    if(this.Worker.CancellationPending)
                    {
                        //this.WorkerEventArgs.Cancel = true;
                        return;
                    }

                    if(this.Option.FileListMode == false)
                    {
                        // GREPモード

                        if(this.Option.ExcludeHidden)
                        {
                            if((subDir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                // 隠しディレクトリーは無視
                                continue;
                            }
                        }

                        if(Patterns.IsExcludeDirectories(subDir.Name))
                        {
                            // #が指定されたディレクトリーは無視
                            continue;
                        }
                    }
                    else
                    {
                        // ファイル一覧モード
                        if(Patterns.IsTargetDirectory(subDir))
                        {
                            collectDirectory(subDir, results);
                        }
                    }

                    // サブディレクトリー毎に検索(再帰)
                    searchDirectory(subDir, results);
                }
            }
            catch(UnauthorizedAccessException ex)
            {
                handlingResultError(results, null, ex);
            }
            catch(PathTooLongException ex)
            {
                handlingResultError(results, null, ex);
            }
        }

        /// <summary>
        /// ファイル内からキーワードを検索して結果を返す
        /// </summary>
        /// <param name="file">ファイル</param>
        /// <param name="results">検索結果</param>
        private void searchKeyword(FileInfo file, ResultContainer results)
        {
            if(this.Worker.CancellationPending)
            {
                //this.WorkerEventArgs.Cancel = true;
                return;
            }

            FileStream fs = null;

            try
            {
                fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            }
            catch(IOException ex)
            {
                if(fs != null)
                {
                    fs.Dispose();
                    fs = null;
                }

                // 他プロセスが読み書き禁止でファイルを開いている場合もあるので
                // 例外情報を残してスキップする
                handlingResultError(results, file, ex);
                return;
            }

            try
            {
                if(this.Option.WordExcel)
                {
                    switch(file.Extension)
                    {
                        case Const.EXT_XLS:
                        case Const.EXT_XLSX:
                        case Const.EXT_DOC:
                        case Const.EXT_DOCX:
                            // TODO:サブスレッド化
                            results.Merge(MsOfficeHelper.SearchKeyword(file, this.Keyword, this.Option));
                            return;
                        default:
                            break;
                    }
                }

                var isBin = Utils.IsBinaryFile(fs);

                // バイナリーファイルを除外?
                if(Option.ExcludeBinaryFile)
                {
                    if(string.IsNullOrEmpty(file.Extension)      // 拡張子がないファイルはバイナリーファイルと見なす!
                    　　|| isBin)
                    {
                        return;
                    }
                }
                fs.Position = 0;

                // ファイルの内容をメモリーに展開
                var allLines = new List<string>();
                var enc = Encoding.GetEncoding("Shift_JIS");

                if(!isBin)
                {
                    enc = Utils.GetEncode(fs);
                    fs.Position = 0;

                    using(var sr = new StreamReader(fs, enc))
                    {
                        allLines = sr.ReadAllLines().ToList();
                    }
                }
                else
                {
                    allLines = Utils.ConvertBinaryToString(fs);
                }

                if(allLines == null || allLines.Count == 0)
                {
                    return;
                }

                dynamic result = null;

                if(this._regExForKeyword == null)
                {
                    if(this.Option.IgnoreCase)
                    {
                        // 大文字小文字を区別しない
                        result = allLines
                                    .Select((s, i) => new { Index = i, Value = s })
                                    .Where(s =>
                                    {
                                        var upper = s.Value.ToUpper();
                                        return upper.Contains(this.Keyword.ToUpper());
                                    });
                    }
                    else
                    {
                        // 大文字小文字を区別する
                        result = allLines
                                    .Select((s, i) => new { Index = i, Value = s })
                                    .Where(s => s.Value.Contains(this.Keyword));
                    }
                }
                else
                {
                    result = allLines
                                .Select((s, i) => new { Index = i, Value = s })
                                .Where(s => this._regExForKeyword.IsMatch(s.Value));
                }

                results.Merge(convertResult(result, file));
            }
            finally
            {
                this.Worker.ReportProgress(results.Results.Count);

                if(fs != null)
                {
                    fs.Dispose();
                    fs = null;
                }
            }
        }

        /// <summary>
        /// ファイル収集
        /// </summary>
        /// <param name="file">ファイル</param>
        /// <param name="results">結果コンテナー</param>
        private void collectFile(FileInfo file, ResultContainer results)
        {
            if(this.Worker.CancellationPending)
            {
                //this.WorkerEventArgs.Cancel = true;
                return;
            }

            var result = new Result(file, 0, file.Name);

            results.Add(result);

            this.Worker.ReportProgress(results.Results.Count);
        }

        /// <summary>
        /// ディレクトリー収集
        /// </summary>
        /// <param name="dir">ディレクトリー</param>
        /// <param name="results">結果コンテナー</param>
        private void collectDirectory(DirectoryInfo dir, ResultContainer results)
        {
            if(this.Worker.CancellationPending)
            {
                //this.WorkerEventArgs.Cancel = true;
                return;
            }

            var result = new Result(dir, dir.Name);

            results.Add(result);

            this.Worker.ReportProgress(results.Results.Count);
        }

        private ResultContainer convertResult(dynamic rawResult, FileInfo file)
        {
            var results = new ResultContainer();

            foreach(var line in rawResult)
            {
                var result = new Result(
                                file,
                                line.Index+1,
                                line.Value);
                results.Add(result);
            }
            return results;
        }

        private void handlingResultError(ResultContainer results, FileInfo file, Exception ex)
        {
            var result = new Result(
                            file == null ? null : file,
                            0,
                            string.Format(MSG_SEARCH_FAILED, ex.Message), false);
            results.Add(result);
        }
    }
}
