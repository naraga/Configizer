using System.Xml.Serialization;

namespace ConfigizerLib.Compilation.Xml
{
    public abstract class Action
    {
    }

    public class Detokenize : Action
    {
        [XmlAttribute]
        public string Paths { get; set; }
    }
}