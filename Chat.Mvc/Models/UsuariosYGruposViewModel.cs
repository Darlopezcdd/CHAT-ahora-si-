using chat.Modelos;

namespace Chat.Mvc.Models
{
    public class UsuariosYGruposViewModel
    {
        public IEnumerable<User> Usuarios { get; set; }
        public IEnumerable<Grupo> Grupos { get; set; }
    }
}

