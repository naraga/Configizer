using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConfigizerLib
{
    public class ConfigurationLoader
    {
        public static ConfigurationFileInfo Load(string path)
        {
            var ext = Path.GetExtension(path) ?? "";
            var dir = Path.GetDirectoryName(path) ?? "";
            var fileName = Path.GetFileName(path) ?? "";
            var cfgName = fileName.Replace(ext, "");
            var contents = File.ReadAllText(path);
            var cfi = new ConfigurationFileInfo
            {
                Name = cfgName,
                Directory = dir,
                ReferencedAssemblies = GetTagValues(contents, "r").Distinct().ToArray(),
                NamespaceImports = GetTagValues(contents, "import").Distinct().ToArray(),
            };

            var baseTags = GetTagValues(contents, "base");
            if (baseTags.Count() > 1)
                throw new Exception("Multiple base configurations specified for " + cfgName);

            if (baseTags.Any())
                cfi.Base = Load(Path.Combine(dir, baseTags.Single().Trim() + ext));

            cfi.Contents = RemoveTags(contents, "base", "r", "import");

            return cfi;
        }

        private static string RemoveTags(string contents, params string[] tags)
        {
            return Regex.Replace(contents, @"(^#(" + string.Join("|", tags) + @")\s+(?<val>.*)\s+$)", "",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        static string[] GetTagValues(string contents, string tagName)
        {
            var tags = Regex.Matches(contents, @"(^#" + tagName + @"\s+(?<val>.*)\s+$)", 
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return tags.OfType<Match>().Select(m => m.Groups["val"].Value).ToArray();
        }
    }
}