using System.Collections.Generic;
using System.Linq;
using ConfigizerLib;
using ConfigizerLib.Compilation;

namespace Configizer
{
    public class Program
    {
        /// <summary>
        /// Applies conciguration. 
        /// </summary>
        /// <param name="cfgPath">Path to configuration file.</param>
        /// <param name="overidenParams">List of overiding parameters in format p1=val2;p2=val2;...</param>
        public static void Apply(string cfgPath, string overidenParams = null)
        {
            var cfgInfo = ConfigurationLoader.Load(cfgPath);
            var compiler = new CsharpConfigurationCompiler();
            var cfg = compiler.Compile(cfgInfo, GetOveridenParams(overidenParams));
            cfg.Apply();
        }

        private static Dictionary<string, string> GetOveridenParams(string overidenParams)
        {
            if (string.IsNullOrWhiteSpace(overidenParams))
                return null;

            return overidenParams.Split(';')
                .Select(pv=>pv.Split('='))
                .ToDictionary(pv => pv[0].Trim(), pv=>pv[1].Trim());
        }

        static void Main(string[] args)
        {
            var cfgInfo = ConfigurationLoader.Load("borko.csconfig");
            var compiler = new CsharpConfigurationCompiler();
            var cfg = compiler.Compile(cfgInfo, null);
            cfg.Apply();
        }
    }
}
