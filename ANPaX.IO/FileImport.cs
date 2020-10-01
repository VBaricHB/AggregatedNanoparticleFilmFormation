using System;
using System.Collections.Generic;
using System.IO;

using ANPaX.Core;
using ANPaX.IO.interfaces;

namespace ANPaX.IO
{
    public class FileImport
    {
        private readonly ISerializer _xmlSerializer;
        private readonly ISerializer _jsonSerializer;
        private readonly ISerializer _lammpsDumpSerializer;

        public FileImport()
        {
            _xmlSerializer = new ANPaXXmlSerializer();
            _jsonSerializer = new ANPaXJsonSerializer();
            _lammpsDumpSerializer = new ANPaXLammpsDumpSerializer(new AccordNeighborslistFactory());
        }

        public IEnumerable<Aggregate> GetAggregatsFromFile(string filename)
        {
            var input = DeserializeFile<AggregateOutput>(filename);
            if (input == default)
            {
                throw new ArgumentException($"Could not deserialize {filename}");
            }
            return input.Aggregates;
        }

        private T DeserializeFile<T>(string filename)
        {
            return (GetFileFormat(filename)) switch
            {
                FileFormat.Xml => _xmlSerializer.DeserializeFile<T>(filename),
                FileFormat.Json => _jsonSerializer.DeserializeFile<T>(filename),
                FileFormat.LammpsDump => _lammpsDumpSerializer.DeserializeFile<T>(filename),
                _ => default,
            };
        }


        private static FileFormat GetFileFormat(string filename)
        {
            return Path.GetExtension(filename) switch
            {
                ".xml" => FileFormat.Xml,
                ".json" => FileFormat.Json,
                ".trj" => FileFormat.LammpsDump,
                ".lammpsdump" => FileFormat.LammpsDump,
                _ => FileFormat.None,

            };
        }
    }

}

