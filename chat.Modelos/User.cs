using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.Modelos
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Name { get; set; }
        public string? ProfilePicture { get; set; }
        public List<Mensaje>? MensajesEnviados { get; set; }
        public List<Mensaje>? MensajesRecibidos { get; set; }
        public List<Grupo>? Grupos { get; set; }
    }
}
