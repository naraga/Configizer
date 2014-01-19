namespace ConfigizerLib
{
    public class ConfigurationFileInfo
    {
        public string[] ReferencedAssemblies { get; set; }
        public string[] NamespaceImports { get; set; }
        public ConfigurationFileInfo Base { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Contents { get; set; }
    }
}