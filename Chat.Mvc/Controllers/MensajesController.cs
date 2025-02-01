using Chat.Mvc.Proxies;
using chat.Modelos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Chat.Mvc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chat.Mvc.Controllers
{
    public class MensajesController : Controller
    {
        private readonly IChatApiProxy _chatApiProxy;

        public MensajesController(IChatApiProxy chatApiProxy)
        {
            _chatApiProxy = chatApiProxy;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int UserRemitenteId)
        {
            var usuarios = await _chatApiProxy.GetUsersAsync();
            var grupos = await _chatApiProxy.GetGruposAsync();

            ViewBag.UserRemitenteId = new SelectList(usuarios, "Id", "Name", UserRemitenteId);
            ViewBag.UserDestinatarioId = new SelectList(usuarios, "Id", "Name");
            ViewBag.GrupoId = new SelectList(grupos, "Id", "Name");

            var mensaje = new Mensaje
            {
                UserRemitenteId = UserRemitenteId
            };

            return View(mensaje);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index(int usuarioActivoId = 0, int? usuarioSeleccionadoId = null, int? grupoId = null)
        {
            if (usuarioActivoId == 0)
            {
                usuarioActivoId = 1; // Usuario por defecto
            }

            var chatViewModel = new ChatViewModel();

            // Obtener todos los datos
            var usuarios = await _chatApiProxy.GetUsersAsync();
            var grupos = await _chatApiProxy.GetGruposAsync();

            var mensajes = await _chatApiProxy.GetMensajesAsync();

            // Filtrar grupos en los que el usuario activo es miembro
            var gruposFiltrados = grupos
                .Where(g => g.Users != null && g.Users.Any(u => u.Id == usuarioActivoId))
                .ToList();


            chatViewModel.Usuarios = usuarios; // Todos los usuarios
            chatViewModel.Grupos = gruposFiltrados; // Grupos filtrados según el usuario activo
            chatViewModel.UsuarioActivoId = usuarioActivoId;
            chatViewModel.UsuarioSeleccionadoId = usuarioSeleccionadoId;

            // Filtrar mensajes
            List<Mensaje> mensajesFiltrados;
            if (grupoId.HasValue)
            {
                mensajesFiltrados = mensajes.Where(m => m.GrupoId == grupoId.Value).ToList();
            }
            else if (usuarioSeleccionadoId.HasValue)
            {
                mensajesFiltrados = mensajes
                    .Where(m =>
                        (m.UserRemitenteId == usuarioActivoId && m.UserDestinatarioId == usuarioSeleccionadoId) ||
                        (m.UserRemitenteId == usuarioSeleccionadoId && m.UserDestinatarioId == usuarioActivoId))
                    .ToList();
            }
            else
            {
                mensajesFiltrados = new List<Mensaje>();
            }

            // Mapear mensajes
            chatViewModel.Mensajes = mensajesFiltrados
                .Select(m => new MensajeConNombres
                {
                    Id = m.Id,
                    Contenido = m.Contenido,
                    FechaEnvio = m.FechaEnvio,
                    NombreRemitente = usuarios.FirstOrDefault(u => u.Id == m.UserRemitenteId)?.Name ?? "Desconocido",
                    NombreDestinatario = m.GrupoId != null
                        ? grupos.FirstOrDefault(g => g.Id == m.GrupoId)?.Name ?? "Grupo Desconocido"
                        : usuarios.FirstOrDefault(u => u.Id == m.UserDestinatarioId)?.Name ?? "Desconocido",
                    UrlArchivo = m.UrlArchivo
                })
                .ToList();

            Console.WriteLine($"Total Grupos: {grupos.Count}");
            foreach (var grupo in grupos)
            {
                Console.WriteLine($"Grupo: {grupo.Name}, Usuarios: {grupo.Users?.Count ?? 0}");
            }
            var leer = await _chatApiProxy.MarcarMensajesComoLeidosAsync(mensajesFiltrados);

            return View(chatViewModel);
        }


        // Métodos corregidos para evitar rutas ambiguas
        [HttpGet("Mensajes/Create/{remitenteId}/{grupoId?}")]
        public async Task<IActionResult> Create(int remitenteId, int? grupoId = null)
        {
            var usuarios = await _chatApiProxy.GetUsersAsync();
            var grupos = await _chatApiProxy.GetGruposAsync();

            List<User> remitentesFiltrados = grupoId.HasValue
                ? grupos.FirstOrDefault(g => g.Id == grupoId)?.Users ?? new List<User>()
                : usuarios;

            ViewBag.UserRemitenteId = new SelectList(remitentesFiltrados, "Id", "Name");
            ViewBag.UserDestinatarioId = grupoId.HasValue ? null : new SelectList(usuarios, "Id", "Name");
            ViewBag.GrupoId = new SelectList(grupos, "Id", "Name", grupoId);

            return View(new Mensaje { UserRemitenteId = remitenteId, GrupoId = grupoId });
        }

        [HttpPost("Mensajes/Create")]
        public async Task<IActionResult> Create(Mensaje mensaje, IFormFile? archivoAdjunto)
        {
            if (mensaje.GrupoId == null && mensaje.UserDestinatarioId == null)
            {
                ModelState.AddModelError("", "Debe seleccionar un destinatario o grupo.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (archivoAdjunto != null && archivoAdjunto.Length > 0)
                    {
                        mensaje.UrlArchivo = await GuardarArchivo(archivoAdjunto);
                    }

                    mensaje.FechaEnvio = DateTime.Now;
                    if (await _chatApiProxy.CreateMensajeAsync(mensaje))
                    {
                        return RedirectToAction("Index", new { usuarioActivoId = mensaje.UserRemitenteId, grupoId = mensaje.GrupoId });
                    }

                    ModelState.AddModelError("", "Error al enviar el mensaje.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            return await RegenerarVistaCreate(mensaje);
        }

        [HttpGet("Mensajes/CreateGrupo/{remitenteId}")]
        public async Task<IActionResult> CreateGrupo(int remitenteId)
        {
            var usuarios = await _chatApiProxy.GetUsersAsync();
            var grupos = await _chatApiProxy.GetGruposAsync();

            ViewBag.UserRemitenteId = new SelectList(usuarios, "Id", "Name");
            ViewBag.GrupoId = new SelectList(grupos, "Id", "Name");

            return View(new Mensaje { UserRemitenteId = remitenteId });
        }

        [HttpPost("Mensajes/CreateGrupo")]
        public async Task<IActionResult> CreateGrupo(Mensaje mensaje, IFormFile? archivoAdjunto)
        {
            if (mensaje.GrupoId == null)
            {
                ModelState.AddModelError("", "Debe seleccionar un grupo.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (archivoAdjunto != null && archivoAdjunto.Length > 0)
                    {
                        mensaje.UrlArchivo = await GuardarArchivo(archivoAdjunto);
                    }

                    mensaje.FechaEnvio = DateTime.Now;
                    if (await _chatApiProxy.CreateMensajeAsync(mensaje))
                    {
                        return RedirectToAction("Index", new { usuarioActivoId = mensaje.UserRemitenteId, grupoId = mensaje.GrupoId });
                    }

                    ModelState.AddModelError("", "Error al enviar el mensaje.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                }
            }

            return await RegenerarVistaCreateGrupo(mensaje);
        }

        private async Task<IActionResult> RegenerarVistaCreate(Mensaje mensaje)
        {
            var usuarios = await _chatApiProxy.GetUsersAsync();
            ViewBag.UserRemitenteId = new SelectList(usuarios, "Id", "Name", mensaje.UserRemitenteId);
            ViewBag.UserDestinatarioId = new SelectList(usuarios, "Id", "Name", mensaje.UserDestinatarioId);
            return View(mensaje);
        }

        private async Task<IActionResult> RegenerarVistaCreateGrupo(Mensaje mensaje)
        {
            var usuarios = await _chatApiProxy.GetUsersAsync();
            var grupos = await _chatApiProxy.GetGruposAsync();
            ViewBag.UserRemitenteId = new SelectList(usuarios, "Id", "Name", mensaje.UserRemitenteId);
            ViewBag.GrupoId = new SelectList(grupos, "Id", "Name", mensaje.GrupoId);
            return View(mensaje);
        }

        private async Task<string> GuardarArchivo(IFormFile archivoAdjunto)
        {
            var uploadPath = Path.Combine("wwwroot/uploads/mensajes");
            var fileName = await FileHelper.SaveFileAsync(archivoAdjunto, uploadPath);
            return $"/uploads/mensajes/{fileName}";
        }
    }
}
