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