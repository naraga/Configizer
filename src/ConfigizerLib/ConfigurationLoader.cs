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
            const string baseTag = "base";
            const string importTag = "import";
            const string referenceTag = "r";
            var allTags = new[] {baseTag, importTag, referenceTag};

            var ext = Path.GetExtension(path) ?? "";
            var dir = Path.GetDirectoryName(path) ?? "";
            var fileName = Path.GetFileName(path) ?? "";
            var cfgName = fileName.Replace(ext, "");
            var contents = File.ReadAllText(path);
            var cfi = new ConfigurationFileInfo
            {
                Name = cfgName,
                Directory = dir,
                ReferencedAssemblies = GetTagValues(contents, referenceTag).Distinct().ToArray(),
                NamespaceImports = GetTagValues(contents, importTag).Distinct().ToArray(),
            };

            var baseConfigs = GetTagValues(contents, baseTag);
            if (baseConfigs.Count() > 1)
                throw new Exception("Multiple base configurations specified for " + cfgName);

            if (baseConfigs.Any())
                cfi.Base = Load(Path.Combine(dir, baseConfigs.Single() + ext));

            cfi.Contents = RemoveTags(contents, allTags);

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
            return tags.OfType<Match>().Select(m => m.Groups["val"].Value.Trim()).ToArray();
        }
    }
}