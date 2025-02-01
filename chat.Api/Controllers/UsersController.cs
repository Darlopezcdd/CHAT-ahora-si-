using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using chat.Modelos;

namespace chat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly dbContext _context;

        public UsersController(dbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // DELETE: api/Users/5

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.User
                    .Include(u => u.MensajesEnviados)
                    .Include(u => u.MensajesRecibidos)
                    
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound("El usuario no existe.");
                }

               
                _context.Mensaje.RemoveRange(user.MensajesEnviados);
                _context.Mensaje.RemoveRange(user.MensajesRecibidos);

                _context.User.Remove(user);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al eliminar el usuario.");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Name))
                {
                    return BadRequest("El nombre del usuario es obligatorio.");
                }

                // Agregar el usuario a la base de datos
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PostUser: {ex.Message}");
                return StatusCode(500, "Error al procesar la solicitud.");
            }
        }





        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
