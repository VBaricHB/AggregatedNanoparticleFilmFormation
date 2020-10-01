using System.Collections.Generic;

using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.Simulation.AggregateFormation.interfaces;

namespace ANPaX.IO
{
    public class FileExport
    {
        private ANPaXJsonSerializer _jsonSerializer;
        private ANPaXXmlSerializer _xmlSerializer;
        private ANPaXLammpsDumpSerializer _lammpsSerializer;

        public FileExport()
        {
            _jsonSerializer = new ANPaXJsonSerializer();
            _xmlSerializer = new ANPaXXmlSerializer();
            _lammpsSerializer = new ANPaXLammpsDumpSerializer(new AccordNeighborslistFactory());
        }

        public void Export(
            List<Aggregate> aggregates,
            IAggregateFormationConfig config,
            string filename,
            FileFormat fileFormat,
            bool doUseSI)
        {

            var output = AggregateOutputMapper.MapToAggregateOutput(aggregates, config, doUseSI);

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

        public void Export(
            IParticleFilm<Aggregate> particleFilm,
            string filename,
            FileFormat fileFormat,
            bool doUseSI)
        {
            switch (fileFormat)
            {
                case FileFormat.Json:
                    _jsonSerializer.Serialize(particleFilm, filename);
                    break;
                case FileFormat.Xml:
                    _xmlSerializer.Serialize(particleFilm, filename);
                    break;
                case FileFormat.LammpsDump:
                    _lammpsSerializer.Serialize(particleFilm, filename);
                    break;
                default:
                    return;
            }
        }





    }
}
