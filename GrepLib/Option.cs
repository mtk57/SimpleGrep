namespace GrepLib
{
    public class Option
    {
        public bool RegExp { get; }
        public bool IgnoreCase { get; }
        public bool ExcludeBinaryFile { get; }
        public bool SearchSubDir { get; }
        public bool ExcludeHidden { get; }
        public bool FileListMode { get; }
        public bool WordExcel { get; }

        private Option()
        {
        }

        public Option(bool regExp,
                      bool ignoreCase, 
                      bool excludeBinaryFile, 
                      bool searchSubDir, 
                      bool excludeHidden,
                      bool fileListMode,
                      bool wordExcel
            )
        {
            this.RegExp = regExp;
            this.IgnoreCase = ignoreCase;
            this.ExcludeBinaryFile = excludeBinaryFile;
            this.SearchSubDir = searchSubDir;
            this.ExcludeHidden = excludeHidden;
            this.FileListMode = fileListMode;
            this.WordExcel = wordExcel;
        }
    }
}
