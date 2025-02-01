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
        [HttpPost]
        public async Task<IActionResult> Create(Mensaje mensaje, IFormFile? archivoAdjunto)
        {
            // Verificar si se ha seleccionado un grupo o un destinatario
            if (mensaje.GrupoId == null && mensaje.UserDestinatarioId == null)
            {
                ModelState.AddModelError("", "Debe seleccionar un destinatario o grupo.");
            }

            // Si el modelo es válido, procesar el mensaje
            if (ModelState.IsValid)
            {
                try
                {
                    // Si se ha adjuntado un archivo, guardarlo
                    if (archivoAdjunto != null && archivoAdjunto.Length > 0)
                    {
                        // Validar tamaño del archivo (Máximo 20MB)
                        if (archivoAdjunto.Length > 20 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "El archivo es demasiado grande. El tamaño máximo permitido es 20MB.");
                            return await RegenerarVistaCreate(mensaje);
                        }

                        mensaje.UrlArchivo = await GuardarArchivo(archivoAdjunto);
                    }

                    // Establecer la fecha de envío del mensaje
                    mensaje.FechaEnvio = DateTime.Now;

                    // Intentar crear el mensaje usando el proxy de la API
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

            // Si el modelo no es válido, regenerar la vista
            return await RegenerarVistaCreate(mensaje);
        }

       

        [HttpGet]
        public async Task<IActionResult> Create(int UserRemitenteId)
        {
            // Obtener usuarios y grupos desde la API
            var usuarios = await _chatApiProxy.GetUsersAsync();
            var grupos = await _chatApiProxy.GetGruposAsync();

            // Pasar los datos a la vista
            ViewBag.UserRemitenteId = new SelectList(usuarios, "Id", "Name", UserRemitenteId);
            ViewBag.UserDestinatarioId = new SelectList(usuarios, "Id", "Name");
            ViewBag.GrupoId = new SelectList(grupos, "Id", "Name");

            var mensaje = new Mensaje
            {
                UserRemitenteId = UserRemitenteId
            };

            return View(mensaje);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(Mensaje mensaje, IFormFile? archivoAdjunto)
        //{
        //    // Verificar si se ha seleccionado un grupo o un destinatario
        //    if (mensaje.GrupoId == null && mensaje.UserDestinatarioId == null)
        //    {
        //        ModelState.AddModelError("", "Debe seleccionar un destinatario o grupo.");
        //    }

        //    // Si el modelo es válido, procesar el mensaje
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Si se ha adjuntado un archivo, guardarlo
        //            if (archivoAdjunto != null && archivoAdjunto.Length > 0)
        //            {
        //                mensaje.UrlArchivo = await GuardarArchivo(archivoAdjunto);
        //            }

        //            // Establecer la fecha de envío del mensaje
        //            mensaje.FechaEnvio = DateTime.Now;

        //            // Intentar crear el mensaje usando el proxy de la API
        //            if (await _chatApiProxy.CreateMensajeAsync(mensaje))
        //            {
        //                return RedirectToAction("Index", new { usuarioActivoId = mensaje.UserRemitenteId, grupoId = mensaje.GrupoId });
        //            }

        //            ModelState.AddModelError("", "Error al enviar el mensaje.");
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", $"Error: {ex.Message}");
        //        }
        //    }

        //    // Si el modelo no es válido, regenerar la vista
        //    return await RegenerarVistaCreate(mensaje);
        //}

        private async Task<IActionResult> RegenerarVistaCreate(Mensaje mensaje)
        {
            var usuarios = await _chatApiProxy.GetUsersAsync();
            var grupos = await _chatApiProxy.GetGruposAsync();

            // Actualizar el ViewBag con los usuarios y grupos
            ViewBag.UserRemitenteId = new SelectList(usuarios, "Id", "Name", mensaje.UserRemitenteId);
            ViewBag.UserDestinatarioId = new SelectList(usuarios, "Id", "Name", mensaje.UserDestinatarioId);
            ViewBag.GrupoId = new SelectList(grupos, "Id", "Name", mensaje.GrupoId);

            return View(mensaje);
        }

        private async Task<string> GuardarArchivo(IFormFile archivoAdjunto)
        {
            var uploadPath = Path.Combine("wwwroot/uploads/mensajes");
            var fileName = await FileHelper.SaveFileAsync(archivoAdjunto, uploadPath);
            return $"/uploads/mensajes/{fileName}";
        }
        [HttpGet]
        public async Task<IActionResult> BuscarMensajesGlobal(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, mensajes = "El parámetro de búsqueda no puede estar vacío." });
            }

            try
            {
                var mensajes = await _chatApiProxy.BuscarMensajesPorContenidoAsync(query);

                if (mensajes.Any())
                {
                    return Json(new { success = true, mensajes });
                }
                else
                {
                    return Json(new { success = false, mensajes = "No se encontraron mensajes." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensajes = $"Error al realizar la búsqueda: {ex.Message}" });
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var mensaje = await _chatApiProxy.GetMensajeByIdAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }
            return View(mensaje);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Mensaje mensaje)
        {
            if (ModelState.IsValid)
            {
                var success = await _chatApiProxy.UpdateMensajeAsync(mensaje);
                if (success)
                {
                    return RedirectToAction("Index", new { usuarioActivoId = mensaje.UserRemitenteId });
                }
                ModelState.AddModelError("", "Error al actualizar el mensaje.");
            }
            return View(mensaje);
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var mensaje = await _chatApiProxy.GetMensajeByIdAsync(id);
            if (mensaje == null)
            {
                return NotFound();
            }
            return View(mensaje);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _chatApiProxy.DeleteMensajeAsync(id);
            if (success)
            {
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Error al eliminar el mensaje.";
            return RedirectToAction("ConfirmDelete", new { id });
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
    }
    }
