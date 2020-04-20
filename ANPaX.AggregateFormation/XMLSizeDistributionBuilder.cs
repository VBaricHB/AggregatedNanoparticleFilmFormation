using System.IO;
using System.Xml.Serialization;

namespace ANPaX.AggregateFormation
{
    internal static class XMLSizeDistributionBuilder<T>
    {

        public static XMLSizeDistribution<T> Read(string file)
        {
            var xRoot = new XmlRootAttribute
            {
                ElementName = "sizes",
                IsNullable = true
            };

            var serializer = new XmlSerializer(typeof(XMLSizeDistribution<T>), xRoot);
            var xmlStream = new FileStream(file, FileMode.Open);

            return (XMLSizeDistribution<T>)serializer.Deserialize(xmlStream);
        }
    }
}
