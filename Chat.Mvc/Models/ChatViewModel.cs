using chat.Modelos;

namespace Chat.Mvc.Models
{
    public class ChatViewModel
    {
        public int UsuarioActivoId { get; set; }
        public int? UsuarioSeleccionadoId { get; set; } 
        public List<User> Usuarios { get; set; } = new List<User>();
        public List<Grupo> Grupos { get; set; } = new List<Grupo>();
        public List<MensajeConNombres> Mensajes { get; set; } = new List<MensajeConNombres>();

    }
    public class MensajeConNombres
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string NombreRemitente { get; set; } 
        public string NombreDestinatario { get; set; }
        public string? UrlArchivo { get; set; }

    }

}
