using System.CodeDom.Compiler;
using System.Collections.Generic;
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

        protected override string GetCompleteConfigClassCode(string originalContent,
            string className, string baseClassName, bool @abstract, IEnumerable<string> nsImports)
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
            return string.Format("public override string {0} {{get{{return \"{1}\";}}}}", propertyName, value);
        }
    }
}