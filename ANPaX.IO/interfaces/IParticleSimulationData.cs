using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IParticleSimulationData
    {
        Task<int> CreateParticleSimulation(ParticleSimulationDTO particleSimulation);
        Task<int> DeleteParticleSimulation(int particleSimulationId);
        Task<List<ParticleSimulationDTO>> GetParticleSimulations();
        Task<ParticleSimulationDTO> GetParticleSimulationById(int particleSimulationId);
        Task<List<ParticleSimulationDTO>> GetParticleSimulationByUser(string user);
        Task<int> UpdateParticleSimulationPercentage(int particleSimulationId, double percentage);
        Task<int> UpdateParticleSimulationStatus(int particleSimulationId, string status);
    }
}
