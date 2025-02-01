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
    public class GrupoesController : ControllerBase
    {
        private readonly dbContext _context;

        public GrupoesController(dbContext context)
        {
            _context = context;
        }

        // GET: api/Grupoes
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grupo>>> GetGrupo()
        {
            return await _context.Grupo
                .Include(g => g.Users) 
                .ToListAsync();
        }


        // GET: api/Grupoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrupo(int id)
        {
            var grupo = await _context.Grupo
                .Include(g => g.Users)
                .Include(g => g.Mensajes)
                .Where(g => g.Id == id)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.FechaCreacion,
                    Users = g.Users.Select(u => new { u.Id, u.Name }),
                    Mensajes = g.Mensajes.Select(m => new { m.Id, m.Contenido, m.FechaEnvio })
                })
                .FirstOrDefaultAsync();

            if (grupo == null)
            {
                return NotFound();
            }

            return Ok(grupo);
        }



        // PUT: api/Grupoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGrupo(int id, Grupo grupo)
        {
            if (id != grupo.Id)
            {
                return BadRequest();
            }

            _context.Entry(grupo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GrupoExists(id))
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

        // POST: api/Grupoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostGrupo(Grupo grupo)
        {
            try
            {
                if (grupo.FechaCreacion == DateTime.MinValue)
                {
                    grupo.FechaCreacion = DateTime.Now; 
                }

          
                if (grupo.Users != null && grupo.Users.Any())
                {
                    var userIds = grupo.Users.Select(u => u.Id).ToList();
                    var usuariosExistentes = _context.User
                        .Where(u => userIds.Contains(u.Id))
                        .ToList();
                    grupo.Users = usuariosExistentes;
                }

                _context.Grupo.Add(grupo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGrupo), new { id = grupo.Id }, grupo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }





        // DELETE: api/Grupoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrupo(int id)
        {
            var grupo = await _context.Grupo.FindAsync(id);
            if (grupo == null)
            {
                return NotFound();
            }

            _context.Grupo.Remove(grupo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GrupoExists(int id)
        {
            return _context.Grupo.Any(e => e.Id == id);
        }
    }
}
