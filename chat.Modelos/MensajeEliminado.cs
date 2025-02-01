using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace chat.Modelos
{
    public class MensajeEliminado
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Mensaje Mensaje { get; set; }
        public DateTime FechaEliminacion { get; set; }= DateTime.Now;

    }
}
