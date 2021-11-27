using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Data;
using ANPaX.IO.DTO;

using Microsoft.EntityFrameworkCore;

namespace ANPaX.Backend
{
    public class ParticleSimulationStorageHelper : IDataStorageHelper<ParticleSimulationDTO>
    {
        private readonly DataContext _context;

        public ParticleSimulationStorageHelper(DataContext context)
        {
            _context = context;
        }
        public async Task<ParticleSimulationDTO> SaveIfNotExist(ParticleSimulationDTO dto)
        {
            var sims = await _context.ParticleSimulations
                .ToListAsync();
            var match = sims.FirstOrDefault(c => c == dto);

            if (match != null)
            {
                return match;
            }
            else
            {
                _context.ParticleSimulations.Add(dto);
                await _context.SaveChangesAsync();
                return dto;
            }
        }
    }
}
