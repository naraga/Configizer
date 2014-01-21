using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigizerLib.Compilation
{
    //TODO: refactor - most of actual Compile() logic could be extracted to support other implementations
    // and actual compilation (GetConfigClassCsharpCode or CreateProvider("CSharp") could be isolated)
    public class CsharpConfigurationCompiler : IConfigurationCompiler
    {
        public ConfigurationBase Compile(ConfigurationFileInfo cfgFileInfo)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var cp = new CompilerParameters
            {
                GenerateInMemory = true
            };
            var configizerLibPath = typeof(IConfigurationCompiler).Assembly.Location;
            cp.ReferencedAssemblies.Add(configizerLibPath);

            var csClasses = new List<string>();
            var actualConfig = cfgFileInfo;
            do
            {
                csClasses.Add(GetConfigClassCsharpCode(
                    actualConfig.Contents,
                    actualConfig.Name,
                    actualConfig.Base != null ? actualConfig.Base.Name : null,
                    new[] { "ConfigizerLib", }));
                actualConfig = actualConfig.Base;
            } while (actualConfig != null);

            var results = provider.CompileAssemblyFromSource(cp, csClasses.ToArray());
            if (results.Errors.HasErrors)
                throw new ConfigurationCompilationException(cfgFileInfo, results.Errors);

            var configTypes = results.CompiledAssembly.GetTypes()
                .Where(t =>
                    typeof(ConfigurationBase).IsAssignableFrom(t) 
                );

            var cfg = (ConfigurationBase)Activator.CreateInstance(configTypes.Single(c=>c.Name == cfgFileInfo.Name));
            cfg.ConfigDirectory = cfgFileInfo.Directory;

            return cfg;
        }

        static string GetConfigClassCsharpCode(string originalContent, 
            string className, string baseClassName, IEnumerable<string> nsImports)
        {
            var cls = new StringBuilder();
            foreach (var ns in nsImports)
            {
                cls.AppendFormat("using {0};", ns);
            }

            cls.AppendFormat("public class @{0}:{1} {{", 
                className,
                !string.IsNullOrWhiteSpace(baseClassName) ? "@" + baseClassName : "ConfigurationBase");
            cls.Append(originalContent);
            cls.Append("}");
            return cls.ToString();
        }
    }
}