using System.Threading.Tasks;

using ANPaX.IO.DTO;
using ANPaX.IO.interfaces;

namespace ANPaX.Backend
{
    public class ParticleSimulationStorageHelper : IDataStorageHelper<ParticleSimulationDTO>
    {
        private readonly IParticleSimulationData _particleSimulationData;

        public ParticleSimulationStorageHelper(IParticleSimulationData particleSimulationData)
        {
            _particleSimulationData = particleSimulationData;
        }
        public async Task<int> SaveIfNotExist(ParticleSimulationDTO dto)
        {
            var id = await _particleSimulationData.CreateParticleSimulation(dto);

            return id;
        }
    }
}
