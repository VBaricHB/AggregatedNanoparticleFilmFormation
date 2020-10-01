using System.Collections.Generic;
using System.Threading.Tasks;

using ANPaX.IO.DTO;

namespace ANPaX.IO.interfaces
{
    public interface IParticleSimulationData
    {
        Task<int> CreateParticleSimulation(ParticleSimulationModel particleSimulation);
        Task<int> DeleteParticleSimulation(int particleSimulationId);
        Task<List<ParticleSimulationModel>> GetParticleSimulations();
        Task<ParticleSimulationModel> GetParticleSimulationById(int particleSimulationId);
        Task<int> UpdateParticleSimulationPercentage(int particleSimulationId, double percentage);
        Task<int> UpdateParticleSimulationStatus(int particleSimulationId, string status);
    }
}
