using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.Modelos
{
    public class Grupo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<Mensaje>? Mensajes { get; set; } = new List<Mensaje>();
        public List<User>? Users { get; set; } = new List<User>();

    }
}
