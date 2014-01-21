using ConfigizerLib;
using ConfigizerLib.Compilation;

namespace Configizer
{

    class Program
    {
        static void Main(string[] args)
        {
            var cfgInfo = ConfigurationLoader.Load("borko.csconfig");
            var compiler = new CsharpConfigurationCompiler();
            var cfg = compiler.Compile(cfgInfo);
            cfg.Apply();
        }
    }
}
