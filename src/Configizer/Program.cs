using System;
using System.Collections.Generic;
using System.Linq;
using ConfigizerLib;
using ConfigizerLib.Compilation;
using ConfigizerLib.Compilation.Xml;

namespace Configizer
{
    public class Program
    {
        /// <summary>
        /// Applies conciguration. 
        /// </summary>
        /// <param name="cfgPath">Path to configuration file.</param>
        /// <param name="overridenParams">List of overiding parameters in format p1=val2;p2=val2;...</param>
        public static void Apply(string cfgPath, string[] overridenParams = null)
        {
            var cfgInfo = ConfigurationLoader.Load(cfgPath);
            IConfigurationCompiler compiler;
            switch (cfgInfo.Language)
            {
                case ConfigLang.Csharp:
                    compiler = new CsharpConfigurationCompiler();
                    break;
                case ConfigLang.Xml:
                    compiler = new XmlConfigurationCompiler();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var cfg = compiler.Compile(cfgInfo, GetOveridenParams(overridenParams));
            cfg.Apply();
        }

        //static void Main(string[] args)
        //{
        //    var cfgInfo = ConfigurationLoader.Load("..\\..\\..\\Samples\\Sample01\\config\\UAT.csconfig");
        //    var compiler = new CsharpConfigurationCompiler();
        //    var cfg = compiler.Compile(cfgInfo, null);
        //    cfg.Apply();
        //}

        private static Dictionary<string, string> GetOveridenParams(string[] overidenParams)
        {
            if (overidenParams == null || !overidenParams.Any())
                return null;

            return overidenParams
                .Select(pv=>pv.Split('='))
                .ToDictionary(pv => pv[0].Trim(), pv=>pv[1].Trim());
        }
    }
}
