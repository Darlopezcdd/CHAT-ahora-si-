using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using chat.Modelos;
using Newtonsoft.Json;

namespace chat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly dbContext _context;

        public MensajesController(dbContext context)
        {
            _context = context;
        }

        // GET: api/Mensajes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetMensaje()
        {
            return await _context.Mensaje.ToListAsync();
        }

        // GET: api/Mensajes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mensaje>> GetMensaje(int id)
        {
            var mensaje = await _context.Mensaje.FindAsync(id);

            if (mensaje == null)
            {
                return NotFound();
            }

            return mensaje;
        }

        // PUT: api/Mensajes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMensaje(int id, Mensaje mensaje)
        {
            if (id != mensaje.Id)
            {
                return BadRequest();
            }

            _context.Entry(mensaje).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MensajeExists(id))
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

        // POST: api/Mensajes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Mensaje>> PostMensaje(Mensaje mensaje)
        {
            Console.WriteLine($"Mensaje recibido: {JsonConvert.SerializeObject(mensaje)}");
            try
            {
                Console.WriteLine($"Mensaje recibido: {System.Text.Json.JsonSerializer.Serialize(mensaje)}");

                // Verificar si UrlArchivo llega correctamente
                if (string.IsNullOrEmpty(mensaje.UrlArchivo))
                {
                    Console.WriteLine("Advertencia: UrlArchivo no se recibió.");
                }

                _context.Mensaje.Add(mensaje);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetMensaje", new { id = mensaje.Id }, mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PostMensaje: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // DELETE: api/Mensajes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMensaje(int id)
        {
            var mensaje = await _context.Mensaje.FindAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }

            _context.Mensaje.Remove(mensaje);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("NuevosMensajes")]
        public ActionResult<bool> NuevosMensajes()
        {
            bool hayMensajesNoLeidos = _context.Mensaje.Any(m => !m.Leido);

            return Ok(hayMensajesNoLeidos);
        }
        [HttpPut("MarcarComoLeidos")]
        public async Task<IActionResult> MarcarComoLeidos([FromBody] List<Mensaje> mensajes)
        {
            try
            {
                foreach (var mensaje in mensajes)
                {
                    var mensajeDb = await _context.Mensaje.FindAsync(mensaje.Id);
                    if (mensajeDb != null)
                    {
                        mensajeDb.Leido = true;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al marcar mensajes como leídos: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPut("MarcarMensajesGrupoComoLeidos/{grupoId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarcarMensajesGrupoComoLeidos(int grupoId)
        {
            if (grupoId <= 0)
            {
                return BadRequest("El ID del grupo proporcionado no es válido.");
            }

            var mensajes = await _context.Mensaje
                .Where(m => m.GrupoId == grupoId && !m.Leido)
                .ToListAsync();

            if (!mensajes.Any())
            {
                return NotFound("No hay mensajes no leídos para este grupo.");
            }

            foreach (var mensaje in mensajes)
            {
                mensaje.Leido = true;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        private bool MensajeExists(int id)
        {
            return _context.Mensaje.Any(e => e.Id == id);
        }

        [HttpGet("BuscarPorContenido")]
        public IActionResult BuscarPorContenido(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { success = false, mensajes = "El parámetro de búsqueda no puede estar vacío." });
            }

            try
            {
             
                var mensajes = _context.Mensaje
                    .AsEnumerable() 
                    .Where(m => !string.IsNullOrEmpty(m.Contenido) &&
                                m.Contenido.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!mensajes.Any())
                {
                    return Ok(new { success = false, mensajes = "No se encontraron mensajes." });
                }

         
                var usuarios = _context.User.ToList();
                var grupos = _context.Grupo.ToList();

                var mensajesConNombres = mensajes.Select(m => new
                {
                    m.Id,
                    m.Contenido,
                    m.FechaEnvio,
                    NombreRemitente = usuarios.FirstOrDefault(u => u.Id == m.UserRemitenteId)?.Name ?? "Desconocido",
                    NombreDestinatario = m.GrupoId.HasValue
                        ? grupos.FirstOrDefault(g => g.Id == m.GrupoId)?.Name ?? "Grupo"
                        : usuarios.FirstOrDefault(u => u.Id == m.UserDestinatarioId)?.Name ?? "Desconocido",
                    m.UrlArchivo
                }).ToList();

                return Ok(new { success = true, mensajes = mensajesConNombres });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, mensajes = $"Error interno: {ex.Message}" });
            }
        }


    }
}
