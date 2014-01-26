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
            string className, string baseClassName, bool @abstract, IEnumerable<string> nsImports);
        
        protected abstract string GetPublicOverrideStringPropertySnippet(
            string propertyName, string value);

        private readonly string[] _standardNamespaces =
        {
            "System", "ConfigizerLib"
        };

        public ConfigurationBase Compile(ConfigurationFileInfo cfgFileInfo, Dictionary<string, string> overidenParams)
        {
            var provider = GetCodeDomProvider();
            var cp = new CompilerParameters{GenerateInMemory = true};
            var configizerLibPath = typeof(IConfigurationCompiler).Assembly.Location;
            cp.ReferencedAssemblies.Add(configizerLibPath);

            var haveOveridenParams = overidenParams != null && overidenParams.Any();

            var classesCode = new List<string>();
            var actualConfig = cfgFileInfo;
            do
            {
                var baseClassName = actualConfig.Base != null ? actualConfig.Base.Name : null;
                var @abstract = haveOveridenParams || actualConfig.Name != cfgFileInfo.Name;
                var nsImports = _standardNamespaces.Union(cfgFileInfo.NamespaceImports).Distinct();
                classesCode.Add(GetCompleteConfigClassCode(
                    actualConfig.Contents,
                    actualConfig.Name,
                    baseClassName,
                    @abstract, 
                    nsImports));
                actualConfig = actualConfig.Base;
            } while (actualConfig != null);

            string finalConfigClassName;

            // overrides configuration with new leaf class which contains string virtual properties
            // (typically sent as commandline arguments to configizer)
            if (haveOveridenParams)
            {
                finalConfigClassName = "__configizerOveridingClass";
                classesCode.Add(
                    GetCompleteConfigClassCode(
                        overidenParams.Aggregate("",
                            (acc, p) => acc + GetPublicOverrideStringPropertySnippet(p.Key, p.Value))
                        , finalConfigClassName,
                        cfgFileInfo.Name,
                        false,
                        _standardNamespaces
                        ));
            }
            else
                finalConfigClassName = cfgFileInfo.Name;

            var results = provider.CompileAssemblyFromSource(cp, classesCode.ToArray());
            if (results.Errors.HasErrors)
                throw new ConfigurationCompilationException(cfgFileInfo, results.Errors);

            var configTypes = results.CompiledAssembly.GetTypes()
                .Where(t =>
                    typeof (ConfigurationBase).IsAssignableFrom(t)
                );

            var cfg = (ConfigurationBase)Activator.CreateInstance(configTypes.Single(c=>c.Name == finalConfigClassName));
            cfg.ConfigDirectory = cfgFileInfo.Directory;

            return cfg;
        }
    }
}