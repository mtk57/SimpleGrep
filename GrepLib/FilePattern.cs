namespace GrepLib
{
    public class FilePattern
    {
        private string _Pattern = string.Empty;
        private string _Extesion = string.Empty;
        private string _DirectoryName = string.Empty;
        private bool _IsExcludeFile = false;
        private bool _IsExcludeDir = false;

        public string Pattern
        {
            get
            {
                return _Pattern;
            }
        }

        public string Extesion
        {
            get
            {
                return _Extesion;
            }
        }

        public string DirectoryName
        {
            get
            {
                return _DirectoryName;
            }
        }

        public bool IsExcludeFile
        {
            get
            {
                return _IsExcludeFile;
            }
        }

        public bool IsExcludeDir
        {
            get
            {
                return _IsExcludeDir;
            }
        }

        private FilePattern()
        {
        }

        /// <summary>
        /// ファイルパターン
        /// </summary>
        /// <param name="pattern">
        /// パターン
        /// 真面目にワイルドカードをサポートすると結構メンドイので以下のみとする
        /// 
        /// *
        /// *.*
        /// *.xxx  (xxxは1文字以上の拡張子を表す)
        /// !*.xxx 
        /// #yyy   (ワイルドカード不可。yyyは1文字以上のディレクトリー名を表す)
        /// 
        /// これらのフォーマットになっているかはあまり厳密にはチェックしない
        /// </param>
        public FilePattern(string pattern)
        {
            var ptn = pattern;

            if(pattern.StartsWith("!"))
            {
                // 除外するファイル
                _IsExcludeFile = true;
                ptn = pattern.Substring(1, pattern.Length - 1);
            }
            else if(pattern.StartsWith("#"))
            {
                // 除外するディレクトリー
                _IsExcludeDir = true;
                ptn = pattern.Substring(1, pattern.Length - 1);
                this._DirectoryName = ptn;
            }

            this._Pattern = ptn;
            if(ptn == "*" || ptn == "*.*")
            {
                this._Extesion = ptn;
            }
            else
            {
                this._Extesion = ptn.Replace("*", "");
            }
        }
    }
}
