﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConfigizerLib
{
    public static class ConfigurationLoader
    {
        public static ConfigurationFileInfo Load(string path)
        {
            const string baseTag = "base";
            const string importTag = "import";
            const string referenceTag = "r";
            const string langTag = "lang";

            var allTags = new[] {baseTag, importTag, referenceTag, langTag};

            var ext = Path.GetExtension(path) ?? "";
            var dir = Path.GetDirectoryName(path) ?? "";
            var fileName = Path.GetFileName(path) ?? "";
            var cfgName = fileName.Replace(ext, "");
            var contents = File.ReadAllText(path);
            var cfi = new ConfigurationFileInfo
            {
                Name = cfgName, //TODO: remove all non alpanumeric characters (to be able to build correct clr type names) 
                Directory = dir,
                ReferencedAssemblies = GetTagValues(contents, referenceTag).ToArray(),
                NamespaceImports = GetTagValues(contents, importTag).ToArray(),
            };

            var baseConfigs = GetTagValues(contents, baseTag);
            if (baseConfigs.Count() > 1)
                throw new Exception("Multiple base configurations specified for " + cfgName);

            if (baseConfigs.Any())
                cfi.Base = Load(Path.Combine(dir, baseConfigs.Single() + ext));

            var lang = GetTagValues(contents, langTag).FirstOrDefault();
            switch (lang)
            {
                case "cs": cfi.Language = ConfigLang.Csharp; break;
                case "xml": cfi.Language = ConfigLang.Xml; break;
                default: cfi.Language = ConfigLang.Csharp; break;
            }

            cfi.Contents = RemoveTags(contents, allTags);

            return cfi;
        }

        public static string RemoveTags(string contents, params string[] tags)
        {
            return Regex.Replace(contents, @"(^#(" + string.Join("|", tags) + @")\s+(?<val>.*)\s*$)", "",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        public static string[] GetTagValues(string contents, string tagName)
        {
            var tags = Regex.Matches(contents, @"(^#" + tagName + @"\s+(?<val>.*)\s*$)", 
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return tags.OfType<Match>().Select(m => m.Groups["val"].Value.Trim()).ToArray();
        }
    }
}