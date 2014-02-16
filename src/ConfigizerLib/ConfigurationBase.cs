using System;
using System.Diagnostics;
using System.IO;
using ConfigizerLib.Detokenizer;

namespace ConfigizerLib
{
    public abstract class ConfigurationBase
    {
        public string ConfigDirectory { get; set; }

        protected DummyConvertibleToAnything ThrowNotOverriden()
        {
            return ThrowNotOverriden(new StackTrace(1).GetFrame(0).GetMethod().Name);
        }

        protected DummyConvertibleToAnything ThrowNotOverriden(string paramName)
        {
            throw new Exception("Parameter " + paramName + " missing. You should either override it in child configuration class or provide it as command line argument.");
        }

        protected string GetFileContent(string path)
        {
            var fullPath = Path.IsPathRooted(path) ? path : Path.Combine(ConfigDirectory, path);
            return File.ReadAllText(fullPath);
        }

        protected void Detokenize(string path)
        {
            Detokenize(path, "token");
        }

        protected void Detokenize(string path, string tokenFileExtension)
        {
            TextFilesDetokenizer.Detokenize(this, ConfigDirectory,
                new[] { path }, new[] { "*." + tokenFileExtension },
                TextFilesDetokenizer.RemoveExtensionOutputFileNameSelector(new[] { "." + tokenFileExtension }));
        }

        public virtual void Apply() { }
    }


    //class DealToTradeServiceProcessor
    //{
    //    void Process(object deal)
    //    {
            
    //    }
    //}


    //static class TradeEvents
    //{
    //    void
    //}
}
