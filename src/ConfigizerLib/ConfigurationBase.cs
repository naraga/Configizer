using ConfigizerLib.Detokenizer;

namespace ConfigizerLib
{
    public abstract class ConfigurationBase
    {
        public string ConfigDirectory { get; set; }

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
}
