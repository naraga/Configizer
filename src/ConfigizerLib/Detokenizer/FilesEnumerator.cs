using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConfigizerLib.Detokenizer
{
    public class FilesEnumerator
    {
        private readonly string _baseDir;
        public string[] SearchPatterns { get; private set; }
        public string[] Paths { get; private set; }

        public FilesEnumerator(string[] paths, string[] searchPatterns, string baseDir)
        {
            _baseDir = baseDir;
            Paths = paths;
            SearchPatterns = searchPatterns;
        }

        public IEnumerable<string> GetFileNames()
        {
            return
                from path in Paths
                let pathToUse = Path.IsPathRooted(path) ? path : Path.Combine(_baseDir, path)
                from pattern in SearchPatterns
                where Directory.Exists(pathToUse)
                from f in Directory.EnumerateFiles(pathToUse, pattern, SearchOption.AllDirectories)
                select f;
        }
    }
}