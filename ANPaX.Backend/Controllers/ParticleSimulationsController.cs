using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ANPaX.IO.DBConnection.Data;
using ANPaX.IO.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ANPaX.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticleSimulationsController : ControllerBase
    {
        private readonly DataContext _context;

        public ParticleSimulationsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ParticleSimulations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticleSimulationDTO>>> GetParticleSimulations()
        {
            return await _context.ParticleSimulations.ToListAsync();
        }

        // GET: api/ParticleSimulations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParticleSimulationDTO>> GetParticleSimulationDTO(int id)
        {
            var particleSimulationDTO = await _context.ParticleSimulations.FindAsync(id);

            if (particleSimulationDTO == null)
            {
                return NotFound();
            }

            return particleSimulationDTO;
        }

        [HttpGet("/running")]
        public async Task<ActionResult<IEnumerable<ParticleSimulationDTO>>> GetRunningParticleSimulations()
        {
            var particleSimulationDTOs = await _context.ParticleSimulations
                    .Where(sim => sim.Status == "running")
                    .ToListAsync();

            if (particleSimulationDTOs == null)
            {
                return NotFound();
            }

            return particleSimulationDTOs;
        }

        [HttpGet("user='{user}'")]
        public async Task<ActionResult<IEnumerable<ParticleSimulationDTO>>> GetParticleSimulationDTO(string user)
        {
            var particleSimulationDTOs = await _context.ParticleSimulations
                    .Where(sim => sim.User == user)
                    .ToListAsync();

            if (!particleSimulationDTOs.Any())
            {
                return NotFound();
            }

            return particleSimulationDTOs;
        }

        [HttpGet("user='{user}'/running")]
        public async Task<ActionResult<IEnumerable<ParticleSimulationDTO>>> GetRunningParticleSimulationDTO(string user)
        {
            var particleSimulationDTOs = await _context.ParticleSimulations
                    .Where(sim => sim.User == user)
                    .Where(sim => sim.Status == "running")
                    .ToListAsync();

            if (!particleSimulationDTOs.Any())
            {
                return NotFound();
            }

            return particleSimulationDTOs;
        }

        // PUT: api/ParticleSimulations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticleSimulationDTO(int id, ParticleSimulationDTO particleSimulationDTO)
        {
            if (id != particleSimulationDTO.Id)
            {
                return BadRequest();
            }

            _context.Entry(particleSimulationDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticleSimulationDTOExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ParticleSimulations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ParticleSimulationDTO>> PostParticleSimulationDTO(ParticleSimulationDTO particleSimulationDTO)
        {
            _context.ParticleSimulations.Add(particleSimulationDTO);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParticleSimulationDTO", new { id = particleSimulationDTO.Id }, particleSimulationDTO);
        }

        // DELETE: api/ParticleSimulations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticleSimulationDTO(int id)
        {
            var particleSimulationDTO = await _context.ParticleSimulations.FindAsync(id);
            if (particleSimulationDTO == null)
            {
                return NotFound();
            }

            _context.ParticleSimulations.Remove(particleSimulationDTO);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParticleSimulationDTOExists(int id)
        {
            return _context.ParticleSimulations.Any(e => e.Id == id);
        }
    }
}
