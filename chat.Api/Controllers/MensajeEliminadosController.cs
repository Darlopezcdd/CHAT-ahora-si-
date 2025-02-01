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
    public class MensajeEliminadosController : ControllerBase
    {
        private readonly dbContext _context;

        public MensajeEliminadosController(dbContext context)
        {
            _context = context;
        }

        // GET: api/MensajeEliminados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensajeEliminado>>> GetNotificacion()
        {
            return await _context.Notificacion.ToListAsync();
        }

        // GET: api/MensajeEliminados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MensajeEliminado>> GetMensajeEliminado(int id)
        {
            var mensajeEliminado = await _context.Notificacion.FindAsync(id);

            if (mensajeEliminado == null)
            {
                return NotFound();
            }

            return mensajeEliminado;
        }

        // PUT: api/MensajeEliminados/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMensajeEliminado(int id, MensajeEliminado mensajeEliminado)
        {
            if (id != mensajeEliminado.Id)
            {
                return BadRequest();
            }

            _context.Entry(mensajeEliminado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MensajeEliminadoExists(id))
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

        // POST: api/MensajeEliminados
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MensajeEliminado>> PostMensajeEliminado(MensajeEliminado mensajeEliminado)
        {
            _context.Notificacion.Add(mensajeEliminado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMensajeEliminado", new { id = mensajeEliminado.Id }, mensajeEliminado);
        }

        // DELETE: api/MensajeEliminados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMensajeEliminado(int id)
        {
            var mensajeEliminado = await _context.Notificacion.FindAsync(id);
            if (mensajeEliminado == null)
            {
                return NotFound();
            }

            _context.Notificacion.Remove(mensajeEliminado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MensajeEliminadoExists(int id)
        {
            return _context.Notificacion.Any(e => e.Id == id);
        }
    }
}
