using System.Collections.Generic;

namespace ConfigizerLib.Compilation
{
    public interface IConfigurationCompiler
    {
        ConfigurationBase Compile(ConfigurationFileInfo cfgFileInfo, Dictionary<string, string> overidenParams);
    }
}
