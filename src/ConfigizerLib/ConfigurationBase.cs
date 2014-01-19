using ConfigizerLib.Detokenizer;

namespace ConfigizerLib
{
    public abstract class ConfigurationBase
    {
        public string ConfigDirectory { get; set; }

        protected void Detokenize(string path)
        {
            TextFilesDetokenizer.Detokenize(this, ConfigDirectory,
                new[] { path }, new []{"*.token"}, 
                TextFilesDetokenizer.RemoveExtensionOutputFileNameSelector(new []{".token"}));
        }

        public virtual void Apply() { }
    }
}
