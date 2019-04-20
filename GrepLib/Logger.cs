using System;
using System.IO;
using System.Text;
using System.Threading;

namespace GrepLib
{
    /// <summary>
    /// ロガークラス
    /// </summary>
    public static class Logger
    {
        private static readonly string FORMAT_YYYYMMDDHHMMSSFFF = "yyyy/MM/dd HH:mm:ss.fff";

        /// <summary>
        /// 出力フォーマット
        /// {0}:出力時間
        /// {1}:スレッドID
        /// {2}:ログの内容
        /// </summary>
        private static readonly string FORMAT = "{0}[{1}]:{2}";

        /// <summary>
        /// 出力フォーマット（例外用）
        /// {0}:例外メッセージ
        /// {1}:スタックトレース
        /// </summary>
        private static readonly string FORMAT_EX = "Message={0}, Stack={1}";

        private static StreamWriter _sw = null;
        private static StringBuilder _sb = null;

        /// <summary>ログ出力の有効/無効</summary>
        public static bool IsEnable = false;


        /// <summary>
        /// ロガー開始
        /// </summary>
        /// <param name="path">ログファイルパス(null or ""の場合はStringBuilderに出力)</param>
        /// <returns>ログ出力の有効/無効を返す</returns>
        public static bool Open(string path = null)
        {
            Close();

            if(!string.IsNullOrEmpty(path))
            {
                _sw = new StreamWriter(path, true);
            }
            else
            {
                _sb = new StringBuilder();
            }
            return IsEnable;
        }

        /// <summary>
        /// ログを出力する
        /// </summary>
        /// <param name="data">出力内容</param>
        public static void Write(string data)
        {
            if(IsEnable == false)
            {
                return;
            }

            var writeData = string.Format(
                                    FORMAT,
                                    DateTime.Now.ToString(FORMAT_YYYYMMDDHHMMSSFFF),
                                    Thread.CurrentThread.ManagedThreadId,
                                    data);
            try
            {
                if(_sw != null)
                {
                    _sw.WriteLine(writeData);
                }
                else if(_sb != null)
                {
                    _sb.AppendLine(writeData);
                }
            }
            catch
            {
                // 無視
            }
        }

        /// <summary>
        /// ログを出力する（例外用）
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        public static void Write(Exception ex)
        {
            if(ex == null || IsEnable == false)
            {
                return;
            }
            var message = string.Format(FORMAT_EX, ex.Message, ex.StackTrace);

            Write(message);
        }

        public static void ClearCache()
        {
            if(_sb == null || IsEnable == false)
            {
                return;
            }

            _sb.Clear();
        }

        public static string GetCache()
        {
            if(_sb == null || IsEnable == false)
            {
                return string.Empty;
            }
            return _sb.ToString();
        }

        public static void WriteCache(string path)
        {
            if(string.IsNullOrEmpty(path) || _sb == null || IsEnable == false)
            {
                return;
            }

            using(var sw = new StreamWriter(path, true))
            {
                try
                {
                    sw.WriteLine(_sb.ToString());
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// ロガー終了
        /// </summary>
        public static void Close()
        {
            if(_sw != null)
            {
                _sw.Dispose();
                _sw = null;
            }
            if(_sb != null)
            {
                _sb.Clear();
                _sb = null;
            }
        }
    }
}
