using System.Collections.Generic;

namespace SimpleGrep
{
    public class SaveData
    {
        public const string FileName = "SaveData.xml";

        public List<string> SearchDirectoryPath;
        public List<string> TargetFile;
        public List<string> Keyword;
        public List<string> OutputDirectoryPath;

        public bool RegExp;
        public bool IgnoreCase;
        public bool ExcludeBinaryFile;
        public bool WriteGrepInfo;
        public bool SearchSubDirectories;
        public bool ExcludeHidden;
        public bool FileListMode;
        public bool WordExcel;

        public SaveData()
        {
            // シリアライズ用にデフォルトコンストラクタが必要
        }

        public SaveData(List<string> searchDirectoryPath,
                        List<string> targetFile,
                        List<string> keyword,
                        List<string> outputDirectoryPath,
                        bool regExp, 
                        bool ignoreCase, 
                        bool excludeBinaryFile,
                        bool writeGrepInfo, 
                        bool searchSubDirectories, 
                        bool excludeHidden,
                        bool fileListMode,
                        bool wordExcel
            )
        {
            this.SearchDirectoryPath = searchDirectoryPath;
            this.TargetFile = targetFile;
            this.Keyword = keyword;
            this.OutputDirectoryPath = outputDirectoryPath;

            this.RegExp = regExp;
            this.IgnoreCase = ignoreCase;
            this.ExcludeBinaryFile = excludeBinaryFile;
            this.WriteGrepInfo = writeGrepInfo;
            this.SearchSubDirectories = searchSubDirectories;
            this.ExcludeHidden = excludeHidden;
            this.FileListMode = fileListMode;
            this.WordExcel = wordExcel;
        }
    }
}
