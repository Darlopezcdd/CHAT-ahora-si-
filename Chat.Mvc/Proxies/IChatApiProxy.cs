using chat.Modelos;
using Chat.Mvc.Models;

namespace Chat.Mvc.Proxies
{
    public interface IChatApiProxy
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(User user);
        Task<bool> CreateGrupoAsync(Grupo grupo);
        Task<List<Mensaje>> GetMensajesAsync();
		Task<bool> CreateMensajeAsync(Mensaje mensaje);
        Task<bool> MarcarMensajesComoLeidosAsync(List<Mensaje> mensajes);
        Task<List<Grupo>> GetGruposAsync();
        Task<bool> MarcarMensajesGrupoComoLeidosAsync(int grupoId);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> DeleteGrupoAsync(int id);
        Task<Mensaje> GetMensajeByIdAsync(int id);
        Task<bool> DeleteMensajeAsync(int id);
        Task<bool> UpdateMensajeAsync(Mensaje mensaje);
        Task<Grupo> GetGrupoByIdAsync(int id);
        Task<List<MensajeConNombres>> BuscarMensajesPorContenidoAsync(string query);

    }
}
