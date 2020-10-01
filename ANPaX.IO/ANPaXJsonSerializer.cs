using System.IO;

using ANPaX.IO.interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ANPaX.IO
{
    internal class ANPaXJsonSerializer : ISerializer
    {
        private JsonSerializer _serializer;

        public ANPaXJsonSerializer()
        {
            _serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializer.Converters.Add(new StringEnumConverter());
        }

        public void Serialize<T>(T output, string filename)
        {
            FileGenerationHelper.GenerateFolder(filename);

            using var file = File.CreateText(filename);
            _serializer.Serialize(file, output);
        }

        public T DeserializeFile<T>(string filename)
        {
            var jsonString = File.ReadAllText(filename);
            var input = System.Text.Json.JsonSerializer.Deserialize<T>(jsonString);

            return input;
        }

    }
}
