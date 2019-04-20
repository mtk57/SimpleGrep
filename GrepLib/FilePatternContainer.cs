using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace GrepLib
{
    public class FilePatternContainer
    {
        public List<FilePattern> FilePatterns { get; }

        private List<Regex> _regExs = null;

        private HashSet<string> _targetFiles = new HashSet<string>();

        private HashSet<string> _excludeFiles = new HashSet<string>();

        private HashSet<string> _excludeDirectories = new HashSet<string>();

        private FilePatternContainer()
        {
        }

        public FilePatternContainer(List<FilePattern> filePatterns)
        {
            this.FilePatterns = filePatterns;

            _regExs = new List<Regex>();

            foreach(var ptn in filePatterns)
            {
                if(ptn.IsExcludeFile)
                {
                    _excludeFiles.Add(ptn.Extesion);
                }
                else if(ptn.IsExcludeDir)
                {
                    _excludeDirectories.Add(ptn.DirectoryName);
                }
                else
                {
                    _targetFiles.Add(ptn.Extesion);
                }

                _regExs.Add(Utils.GetWildCardRegEx(ptn.Pattern));
            }
        }

        public bool IsTargetFile(FileInfo file, bool isFileListMode = false)
        {
            if(isFileListMode)
            {
                foreach(var regEx in _regExs)
                {
                    if(regEx.IsMatch(file.Name))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if(!_targetFiles.Contains(file.Name) &&
                    !_targetFiles.Contains(file.Extension))
                {
                    // 見つからなくてもワイルドカードが指定されているかをチェック
                    if(_targetFiles.Contains("*")
                        || _targetFiles.Contains("*.*"))
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        public bool IsExcludeFile(FileInfo file)
        {
            return _excludeFiles.Contains(file.Name)
                || _excludeFiles.Contains(file.Extension)
                || _excludeFiles.Any(x => file.Name.EndsWith(x));
        }

        public bool IsExcludeDirectories(string directoryName)
        {
            return _excludeDirectories.Contains(directoryName);
        }

        public bool IsTargetDirectory(DirectoryInfo dir)
        {
            foreach(var regEx in _regExs)
            {
                if(regEx.IsMatch(dir.Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
