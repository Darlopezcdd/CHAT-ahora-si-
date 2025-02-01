using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.Modelos
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Mensaje { get; set; }
        public User? User { get; set; }

    }
}
