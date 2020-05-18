using System;
using System.Collections.Generic;
using System.IO;

using ANPaX.Core;
using ANPaX.IO;

namespace ANPaX.IO.Import
{
    public static class FileImport
    {

        public static T DeserializeFile<T>(string filename)
        {
            switch (GetFileFormat(filename))
            {
                case FileFormat.Xml:
                    return XmlDeserializer.DeserializeFile<T>(filename);
                case FileFormat.Json:
                    return JsonDeserializer.DeserializeFile<T>(filename);
                case FileFormat.LammpsDump:
                default:
                    return default;
            }
        }

        public static IEnumerable<Aggregate> GetAggregatsFromFile(string filename)
        {
            var input = DeserializeFile<AggregateOutput>(filename);
            if (input == default)
            {
                throw new ArgumentException($"Could not deserialize {filename}");
            }
            return input.Aggregates;
        }


        private static FileFormat GetFileFormat(string filename)
        {
            var extension = Path.GetExtension(filename);
            switch (extension)
            {
                case ".xml":
                    return FileFormat.Xml;
                case ".json":
                    return FileFormat.Json;
                case ".trj":
                case ".lammpsdump":
                    return FileFormat.LammpsDump;
                default:
                    return FileFormat.None;
            }
        }
    }
}
