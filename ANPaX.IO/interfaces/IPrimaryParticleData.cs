using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IPrimaryParticleData
    {
        Task<int> CreateAggregateConfiguration(PrimaryParticleDTO primaryParticleDTO);
        Task<PrimaryParticleDTO> GetPrimaryParticleById(int primaryParticleId);
        Task<IEnumerable<PrimaryParticleDTO>> GetPrimaryParticlesOfAggregateOfSimulationById(int particleSimulationId, int aggregateId);
        Task<IEnumerable<PrimaryParticleDTO>> GetPrimaryParticlesOfSimulationById(int particleSimulationId);
    }
}
