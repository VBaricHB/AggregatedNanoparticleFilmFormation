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
    public class FilmFormationConfigurationController : ControllerBase
    {
        private readonly DataContext _context;

        public FilmFormationConfigurationController(DataContext context)
        {
            _context = context;
        }

        // GET: api/FilmFormationConfiguration
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FilmFormationConfigurationDTO>>> GetFilmFormationConfigs()
        {
            return await _context.FilmFormationConfigs.ToListAsync();
        }

        // GET: api/FilmFormationConfiguration/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FilmFormationConfigurationDTO>> GetFilmFormationConfigurationDTO(int id)
        {
            var filmFormationConfigurationDTO = await _context.FilmFormationConfigs.FindAsync(id);

            if (filmFormationConfigurationDTO == null)
            {
                return NotFound();
            }

            return filmFormationConfigurationDTO;
        }

        // PUT: api/FilmFormationConfiguration/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilmFormationConfigurationDTO(int id, FilmFormationConfigurationDTO filmFormationConfigurationDTO)
        {
            if (id != filmFormationConfigurationDTO.Id)
            {
                return BadRequest();
            }

            _context.Entry(filmFormationConfigurationDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmFormationConfigurationDTOExists(id))
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

        // POST: api/FilmFormationConfiguration
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FilmFormationConfigurationDTO>> PostFilmFormationConfigurationDTO(FilmFormationConfigurationDTO filmFormationConfigurationDTO)
        {
            _context.FilmFormationConfigs.Add(filmFormationConfigurationDTO);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFilmFormationConfigurationDTO", new { id = filmFormationConfigurationDTO.Id }, filmFormationConfigurationDTO);
        }

        // DELETE: api/FilmFormationConfiguration/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilmFormationConfigurationDTO(int id)
        {
            var filmFormationConfigurationDTO = await _context.FilmFormationConfigs.FindAsync(id);
            if (filmFormationConfigurationDTO == null)
            {
                return NotFound();
            }

            _context.FilmFormationConfigs.Remove(filmFormationConfigurationDTO);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilmFormationConfigurationDTOExists(int id)
        {
            return _context.FilmFormationConfigs.Any(e => e.Id == id);
        }
    }
}
