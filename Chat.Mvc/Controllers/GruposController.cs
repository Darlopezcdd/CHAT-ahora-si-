using chat.Modelos;
using Chat.Mvc.Proxies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Chat.Mvc.Controllers
{
    public class GruposController : Controller
    {
        private readonly IChatApiProxy _chatApiProxy;

        public GruposController(IChatApiProxy chatApiProxy)
        {
            _chatApiProxy = chatApiProxy;
        }

      
        public async Task<IActionResult> Index()
        {
            var grupos = await _chatApiProxy.GetGruposAsync(); 
            return View(grupos);
        }


        public async Task<IActionResult> Create()
        {
            var usuarios = await _chatApiProxy.GetUsersAsync();
            ViewBag.Users = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();

            return View(new Grupo());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Grupo grupo, int[] selectedUsers)
        {
            if (selectedUsers == null || selectedUsers.Length < 3)
            {
                ModelState.AddModelError("", "Debe seleccionar al menos 3 usuarios para el grupo.");
                var usuarios = await _chatApiProxy.GetUsersAsync();
                ViewBag.Users = usuarios.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name
                }).ToList();
                return View(grupo);
            }

            // Filtrar los usuarios seleccionados
            var usuariosCompletos = await _chatApiProxy.GetUsersAsync();
            grupo.Users = usuariosCompletos
                .Where(u => selectedUsers.Contains(u.Id))
                .Select(u => new User
                {
                    Id = u.Id,
                    Name = u.Name
                })
                .ToList();

            // Si hay usuarios seleccionados, deshabilitar el campo de destinatario en la vista
            ViewBag.DestinatarioDeshabilitado = true;

            // Enviar datos a la API
            var success = await _chatApiProxy.CreateGrupoAsync(grupo);
            if (success)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error al crear el grupo.");
            var usuariosParaVista = await _chatApiProxy.GetUsersAsync();
            ViewBag.Users = usuariosParaVista.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();

            return View(grupo);
        }



        public async Task<IActionResult> Details(int id)
        {
       
            var grupo = await _chatApiProxy.GetGrupoByIdAsync(id);
            if (grupo == null)
            {
                return NotFound();
            }

            return View(grupo);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _chatApiProxy.DeleteGrupoAsync(id); 
                if (!success)
                {
                    TempData["Error"] = "No se pudo eliminar el grupo. Verifica si existe.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al intentar eliminar: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
