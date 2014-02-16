using System.Collections.Generic;
using System.Text;

namespace ConfigizerLib.Compilation.Xml
{
    //TODO: suport something like "must ovverride" on xml params and generate ThrowNotOverriden
    public class XmlConfigurationCompiler : CsharpConfigurationCompiler
    {
        protected override string CreateConfigClassCode(string originalContent,
            string className, string baseClassName, bool @abstract, bool isLeaf, IEnumerable<string> nsImports)
        {
            var sb = new StringBuilder();
            var doc = ConfigurationDocument.Parse(originalContent);
            foreach (var p in doc.Parameters)
                sb.AppendLine(GetPropertyForConfigParam(p));

            sb.AppendLine("public override void Apply(){");
            sb.AppendLine("base.Apply();");
            foreach (var action in doc.Actions)
            {
                var detokenize = action as Detokenize;
                if (detokenize != null)
                {
                    foreach (var path in detokenize.Paths.Split(',', ';'))
                    {
                        sb.AppendFormat("Detokenize(@\"{0}\");\n", path);
                    }
                }
            }
            sb.AppendLine("}");
            
            return base.CreateConfigClassCode(sb.ToString(), className, baseClassName, @abstract, isLeaf, nsImports);
        }

        string GetPropertyForConfigParam(ConfigParam p)
        {
            if (p.IsFromFile)
                return GetPublicVirtualStringPropertySnippet(p.Name, "GetFileContent", p.File);
         
            return GetPublicVirtualStringPropertySnippet(p.Name, p.GetValue());
        }
    }
}
