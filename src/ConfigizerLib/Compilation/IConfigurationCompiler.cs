namespace ConfigizerLib.Compilation
{
    public interface IConfigurationCompiler
    {
        ConfigurationBase Compile(ConfigurationFileInfo cfgFileInfo);
    }
}
