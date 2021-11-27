using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ANPaX.IO.DBConnection.Data;
using ANPaX.IO.DTO;

namespace ANPaX.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly DataContext _context;

        public UserDataController(DataContext context)
        {
            _context = context;
        }

        // GET: api/UserData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/UserData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserDTO(int id)
        {
            var userDTO = await _context.Users.FindAsync(id);

            if (userDTO == null)
            {
                return NotFound();
            }

            return userDTO;
        }

        // PUT: api/UserData/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDTO(int id, UserDTO userDTO)
        {
            if (id != userDTO.Id)
            {
                return BadRequest();
            }

            _context.Entry(userDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDTOExists(id))
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

        // POST: api/UserData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUserDTO(UserDTO userDTO)
        {
            _context.Users.Add(userDTO);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserDTO", new { id = userDTO.Id }, userDTO);
        }

        // DELETE: api/UserData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDTO(int id)
        {
            var userDTO = await _context.Users.FindAsync(id);
            if (userDTO == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userDTO);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserDTOExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
