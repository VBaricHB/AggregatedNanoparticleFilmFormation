using System.IO;
using System.Xml.Serialization;

using ANPaX.IO.interfaces;

namespace ANPaX.IO
{
    internal class ANPaXXmlSerializer : ISerializer
    {
        public void Serialize<T>(T output, string filename)
        {
            FileGenerationHelper.GenerateFolder(filename);
            var serializer = new XmlSerializer(typeof(T));
            var writer = new StreamWriter(filename);
            serializer.Serialize(writer, output);
            writer.Close();
        }

        public T DeserializeFile<T>(string filename)
        {
            var deserializer = new XmlSerializer(typeof(T));
            var textReader = new StreamReader(filename);
            return (T)deserializer.Deserialize(textReader);
        }

    }
}
