using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace ConfigizerLib.Compilation
{
    public abstract class ConfigurationCompilerBase : IConfigurationCompiler
    {
        protected abstract CodeDomProvider GetCodeDomProvider();

        protected  abstract string GetCompleteConfigClassCode(string originalContent,
            string className, string baseClassName, IEnumerable<string> nsImports);

        public ConfigurationBase Compile(ConfigurationFileInfo cfgFileInfo)
        {
            var provider = GetCodeDomProvider();
            var cp = new CompilerParameters{GenerateInMemory = true};
            var configizerLibPath = typeof(IConfigurationCompiler).Assembly.Location;
            cp.ReferencedAssemblies.Add(configizerLibPath);

            var classesCode = new List<string>();
            var actualConfig = cfgFileInfo;
            do
            {
                classesCode.Add(GetCompleteConfigClassCode(
                    actualConfig.Contents,
                    actualConfig.Name,
                    actualConfig.Base != null ? actualConfig.Base.Name : null,
                    new[] {"ConfigizerLib"}));
                actualConfig = actualConfig.Base;
            } while (actualConfig != null);

            var results = provider.CompileAssemblyFromSource(cp, classesCode.ToArray());
            if (results.Errors.HasErrors)
                throw new ConfigurationCompilationException(cfgFileInfo, results.Errors);

            var configTypes = results.CompiledAssembly.GetTypes()
                .Where(t =>
                    typeof (ConfigurationBase).IsAssignableFrom(t)
                );

            var cfg = (ConfigurationBase)Activator.CreateInstance(configTypes.Single(c=>c.Name == cfgFileInfo.Name));
            cfg.ConfigDirectory = cfgFileInfo.Directory;

            return cfg;
        }
    }
}