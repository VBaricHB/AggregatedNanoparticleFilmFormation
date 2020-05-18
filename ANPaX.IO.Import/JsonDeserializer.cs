using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using ANPaX.Core;
using ANPaX.IO;

namespace ANPaX.IO.Import
{
    internal static class JsonDeserializer
    {
        public static T DeserializeFile<T>(string filename)
        {
            var jsonString = File.ReadAllText(filename);
            var input = JsonSerializer.Deserialize<T>(jsonString);

            return input;
        }

        public static IEnumerable<Aggregate> GetAggregatsFromFile(string filename)
        {
            var input = DeserializeFile<AggregateOutput>(filename);

            return input.Aggregates;
        }

    }
}
