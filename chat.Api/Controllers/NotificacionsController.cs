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
    public class NotificacionsController : ControllerBase
    {
        private readonly dbContext _context;

        public NotificacionsController(dbContext context)
        {
            _context = context;
        }

        // GET: api/Notificacions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notificacion>>> GetNotificacion()
        {
            return await _context.Notificacion.ToListAsync();
        }

        // GET: api/Notificacions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notificacion>> GetNotificacion(int id)
        {
            var notificacion = await _context.Notificacion.FindAsync(id);

            if (notificacion == null)
            {
                return NotFound();
            }

            return notificacion;
        }

        // PUT: api/Notificacions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotificacion(int id, Notificacion notificacion)
        {
            if (id != notificacion.Id)
            {
                return BadRequest();
            }

            _context.Entry(notificacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificacionExists(id))
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

        // POST: api/Notificacions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Notificacion>> PostNotificacion(Notificacion notificacion)
        {
            _context.Notificacion.Add(notificacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotificacion", new { id = notificacion.Id }, notificacion);
        }

        // DELETE: api/Notificacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotificacion(int id)
        {
            var notificacion = await _context.Notificacion.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }

            _context.Notificacion.Remove(notificacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotificacionExists(int id)
        {
            return _context.Notificacion.Any(e => e.Id == id);
        }
    }
}
