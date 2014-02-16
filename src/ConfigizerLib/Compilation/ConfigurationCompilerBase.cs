using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace ConfigizerLib.Compilation
{
    public abstract class ConfigurationCompilerBase : IConfigurationCompiler
    {
        protected abstract CodeDomProvider GetCodeDomProvider();

        protected abstract string CreateConfigClassCode(string originalContent,
            string className, string baseClassName, bool @abstract, bool isLeaf,
            IEnumerable<string> nsImports);

        protected abstract string CreateClassWithOverridingParams(
            string className, string baseClassName, IEnumerable<string> nsImports,
            Dictionary<string, string> overidenParams);

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
                classesCode.Add(CreateConfigClassCode(
                    actualConfig.Contents,
                    actualConfig.Name,
                    baseClassName,
                    @abstract, // every non-leaf class is abstract
                    !@abstract, 
                    nsImports));
                actualConfig = actualConfig.Base;
            } while (actualConfig != null);

            string leafConfigClassName;

            // overrides configuration with new leaf class which contains string virtual properties
            // (typically sent as commandline arguments to configizer)
            if (haveOveridenParams)
            {
                leafConfigClassName = "__configizerOveridingClass";
                classesCode.Add(CreateClassWithOverridingParams(leafConfigClassName, cfgFileInfo.Name, _standardNamespaces, overidenParams));
            }
            else
                leafConfigClassName = cfgFileInfo.Name;

            var results = provider.CompileAssemblyFromSource(cp, classesCode.ToArray());
            if (results.Errors.HasErrors)
                throw new ConfigurationCompilationException(cfgFileInfo, results.Errors);

            var configTypes = results.CompiledAssembly.GetTypes()
                .Where(t =>
                    typeof (ConfigurationBase).IsAssignableFrom(t)
                );

            var cfg = (ConfigurationBase)Activator.CreateInstance(configTypes.Single(c=>c.Name == leafConfigClassName));
            cfg.ConfigDirectory = cfgFileInfo.Directory;

            return cfg;
        }
    }
}