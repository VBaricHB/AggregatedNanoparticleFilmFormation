using System.IO;
using System.Xml.Serialization;

namespace ANPaX.IO.Export
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

    }
}
