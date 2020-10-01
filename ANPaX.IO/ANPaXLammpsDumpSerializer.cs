
using ANPaX.Core;
using ANPaX.Core.interfaces;
using ANPaX.IO.interfaces;

namespace ANPaX.IO
{
    public class ANPaXLammpsDumpSerializer : ISerializer
    {
        private readonly INeighborslistFactory _neighborslistFactory;
        public ANPaXLammpsDumpSerializer(INeighborslistFactory neighborslistFactory)
        {
            _neighborslistFactory = neighborslistFactory;
        }

        public void Serialize<T>(T output, string filename)
        {
            if (typeof(T) == typeof(IParticleFilm<Aggregate>))
            {
                var film = output as IParticleFilm<Aggregate>;
                LammpsDumpSerializer.SerializeParticleFilm(film, filename);
            }
        }

        public T DeserializeFile<T>(string filename, INeighborslistFactory neighborslistFactory)
        {
            return (T)LammpsDumpDeserializer.DeserializeParticleFilm(filename, neighborslistFactory);
        }

        public T DeserializeFile<T>(string filename)
        {
            return DeserializeFile<T>(filename, _neighborslistFactory);
        }
    }
}
