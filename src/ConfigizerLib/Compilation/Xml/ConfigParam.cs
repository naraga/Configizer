using System.Xml.Serialization;

namespace ConfigizerLib.Compilation.Xml
{
    

    public class ConfigParam
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }

        [XmlAttribute("File")]
        public string File { get; set; }

        [XmlText]
        public string ValueText { get; set; }


        public string GetValue()
        {
            if (!string.IsNullOrEmpty(Value))
                return Value;

            if (!string.IsNullOrEmpty(ValueText))
                return ValueText;

            return string.Empty;
        }

        public bool IsFromFile
        {
            get { return string.IsNullOrEmpty(GetValue()) && !string.IsNullOrEmpty(File); }
        }
    }
}