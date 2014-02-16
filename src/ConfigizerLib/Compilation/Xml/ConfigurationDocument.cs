using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ConfigizerLib.Compilation.Xml
{
    [XmlRoot("Configuration")]
    public class ConfigurationDocument
    {
        [XmlArrayItem("P")]
        public ConfigParam[] Parameters { get; set; }

        [XmlArrayItem(typeof(Detokenize))]
        public Action[] Actions { get; set; }

        public static ConfigurationDocument Parse(string xmlContent)
        {
            var serializer = new XmlSerializer(typeof(ConfigurationDocument));
            var xmlReader = XmlReader.Create(new StringReader(xmlContent));
            var document = (ConfigurationDocument)serializer.Deserialize(xmlReader);
            return document;
        }
    }
}