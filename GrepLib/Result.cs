using System;
using System.IO;

namespace GrepLib
{
    public class Result
    {
        private const string FORMAT = "{0}({1}):{2}";   // {0}:絶対パス, {1}:行番号, {2}:行データ

        public FileInfo File { get; }
        public DirectoryInfo Directory { get; }
        public int LineNumber { get; }
        public string Value { get; }
        public bool IsSuccess { get; }

        private Result()
        {
            // 封印
        }

        public Result(FileInfo file, int lineNumber, string value, bool isSuccess = true)
        {
            this.File = file;
            this.LineNumber = lineNumber;
            this.Value = value;
            this.IsSuccess = isSuccess;
        }

        public Result(DirectoryInfo dir, string value, bool isSuccess = true)
        {
            this.Directory = dir;
            this.LineNumber = 0;
            this.Value = value;
            this.IsSuccess = isSuccess;
        }

        public override string ToString()
        {
            var path = this.File == null ? string.Empty : this.File.FullName;

            if(string.IsNullOrEmpty(path))
            {
                path = this.Directory == null ? string.Empty : this.Directory.FullName;
            }

            return string.Format(FORMAT, path, this.LineNumber, this.Value);
        }
    }
}
