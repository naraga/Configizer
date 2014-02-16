namespace ConfigizerLib
{
    public enum ConfigLang
    {
        Csharp, Xml
    }

    public class ConfigurationFileInfo
    {
        public ConfigurationFileInfo()
        {
            ReferencedAssemblies = new string[] {};
            NamespaceImports = new string[] { };
        }

        public ConfigLang Language { get; set; }
        public string[] ReferencedAssemblies { get; set; }
        public string[] NamespaceImports { get; set; }
        public ConfigurationFileInfo Base { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Contents { get; set; }
    }
}