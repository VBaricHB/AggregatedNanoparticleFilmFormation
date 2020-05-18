using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using ANPaX.Core;
using ANPaX.IO;

namespace ANPaX.IO.Import
{
    internal static class XmlDeserializer
    {
        public static T DeserializeFile<T>(string filename)
        {
            var deserializer = new XmlSerializer(typeof(T));
            var textReader = new StreamReader(filename);
            return (T)deserializer.Deserialize(textReader);
        }

        public static IEnumerable<Aggregate> GetAggregatsFromFile(string filename)
        {
            var input = DeserializeFile<AggregateOutput>(filename);

            return input.Aggregates;
        }
    }
}
