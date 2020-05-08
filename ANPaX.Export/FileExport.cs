using System;
using System.Collections.Generic;

using ANPaX.AggregateFormation.interfaces;
using ANPaX.Collection;

namespace ANPaX.Export
{
    public class FileExport
    {
        private ANPaXJsonSerializer _jsonSerializer;
        private ANPaXXmlSerializer _xmlSerializer;

        public FileExport()
        {
            _jsonSerializer = new ANPaXJsonSerializer();
            _xmlSerializer = new ANPaXXmlSerializer();
        }

        public void Export(
            List<Aggregate> aggregates,
            IAggregateFormationConfig config,
            string filename,
            FileFormat fileFormat,
            bool convertToSI)
        {

            var output = AggregateOutputMapper.MapToAggregateOutput(aggregates, config, convertToSI);

            switch (fileFormat)
            {
                case FileFormat.Json:
                    _jsonSerializer.Serialize(output, filename);
                    break;
                case FileFormat.Xml:
                    _xmlSerializer.Serialize(output, filename);
                    break;

                default:
                    return;
            }
        }

        public void Export(
            List<Aggregate> aggregates,
            IAggregateFormationConfig config,
            string filename,
            FileFormat fileFormat)
        {
            Export(aggregates,
                   config,
                   filename,
                   fileFormat,
                   true);
        }



        private static FileFormat GetFileFormat(string filename)
        {
            if (filename.EndsWith("xml"))
            {
                return FileFormat.Xml;
            }
            else if (filename.EndsWith("json"))
            {
                return FileFormat.Json;
            }
            else if (filename.Contains(".trj") || filename.Contains("lammps"))
            {
                return FileFormat.LammpsDump;
            }

            throw new ArgumentException($"Invalid filetype {filename}.");
        }

    }
}
