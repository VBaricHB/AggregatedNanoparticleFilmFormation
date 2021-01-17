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
    public class AggregateConfigurationController : ControllerBase
    {
        private readonly DataContext _context;

        public AggregateConfigurationController(DataContext context)
        {
            _context = context;
        }

        // GET: api/AggConfig
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AggregateConfigurationDTO>>> GetAggConfigs()
        {
            return await _context.AggConfigs.ToListAsync();
        }

        // GET: api/AggConfig/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AggregateConfigurationDTO>> GetAggregateConfigurationDTO(int id)
        {
            var aggregateConfigurationDTO = await _context.AggConfigs.FindAsync(id);

            if (aggregateConfigurationDTO == null)
            {
                return NotFound();
            }

            return aggregateConfigurationDTO;
        }

        // PUT: api/AggConfig/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAggregateConfigurationDTO(int id, AggregateConfigurationDTO aggregateConfigurationDTO)
        {
            if (id != aggregateConfigurationDTO.Id)
            {
                return BadRequest();
            }

            _context.Entry(aggregateConfigurationDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AggregateConfigurationDTOExists(id))
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

        // POST: api/AggConfig
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AggregateConfigurationDTO>> PostAggregateConfigurationDTO(AggregateConfigurationDTO aggregateConfigurationDTO)
        {
            _context.AggConfigs.Add(aggregateConfigurationDTO);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAggregateConfigurationDTO", new { id = aggregateConfigurationDTO.Id }, aggregateConfigurationDTO);
        }

        // DELETE: api/AggConfig/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAggregateConfigurationDTO(int id)
        {
            var aggregateConfigurationDTO = await _context.AggConfigs.FindAsync(id);
            if (aggregateConfigurationDTO == null)
            {
                return NotFound();
            }

            _context.AggConfigs.Remove(aggregateConfigurationDTO);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AggregateConfigurationDTOExists(int id)
        {
            return _context.AggConfigs.Any(e => e.Id == id);
        }
    }
}
