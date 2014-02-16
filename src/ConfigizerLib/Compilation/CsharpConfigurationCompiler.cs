using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigizerLib.Compilation
{
    //TODO: refactor - most of actual Compile() logic could be extracted to support other implementations
    // and actual compilation (GetConfigClassCsharpCode or CreateProvider("CSharp") could be isolated)

    public class CsharpConfigurationCompiler : ConfigurationCompilerBase
    {
        protected override CodeDomProvider GetCodeDomProvider()
        {
            return CodeDomProvider.CreateProvider("CSharp");
        }

        protected override string CreateConfigClassCode(string originalContent,
            string className, string baseClassName, bool @abstract, bool isLeaf, IEnumerable<string> nsImports)
        {
            return CreateCsharpConfigClassCode(originalContent, className, baseClassName, @abstract, nsImports);
        }

        protected override string CreateClassWithOverridingParams(string className, string baseClassName, IEnumerable<string> nsImports, Dictionary<string, string> overidenParams)
        {
            return
                CreateCsharpConfigClassCode(
                    overidenParams.Aggregate("",
                        (acc, p) => acc + GetPublicOverrideStringPropertySnippet(p.Key, p.Value))
                    , className,
                    baseClassName,
                    false,
                    nsImports
                    );
        }

        private static string CreateCsharpConfigClassCode(string originalContent, string className, string baseClassName,
            bool @abstract, IEnumerable<string> nsImports)
        {
            var cls = new StringBuilder();
            foreach (var ns in nsImports)
            {
                cls.AppendFormat("using {0};", ns);
            }

            cls.AppendFormat("public {0} class @{1}:{2} {{",
                @abstract ? "abstract" : "",
                className,
                !string.IsNullOrWhiteSpace(baseClassName) ? "@" + baseClassName : "ConfigurationBase");
            cls.Append(originalContent);
            cls.Append("}");
            return cls.ToString();
        }

        protected override string GetPublicOverrideStringPropertySnippet(string propertyName, string value)
        {
            return string.Format("public override string {0} {{get{{return @\"{1}\";}}}}", propertyName, value);
        }

        protected string GetPublicOverrideStringPropertySnippet(string propertyName, string appliedFnName,string value)
        {
            return string.Format("public override string {0} {{get{{return {1}(@\"{2}\");}}}}", propertyName, appliedFnName, value);
        }

        protected string GetPublicVirtualStringPropertySnippet(string propertyName, string value)
        {
            return string.Format("public virtual string {0} {{get{{return @\"{1}\";}}}}", propertyName, value);
        }

        protected string GetPublicVirtualStringPropertySnippet(string propertyName, string appliedFnName, string value)
        {
            return string.Format("public virtual string {0} {{get{{return {1}(@\"{2}\");}}}}", propertyName, appliedFnName, value);
        }
    }
}