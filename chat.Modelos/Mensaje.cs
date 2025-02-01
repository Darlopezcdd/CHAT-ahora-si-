using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace chat.Modelos
{
    public class Mensaje
    {
        public int Id { get; set; }

        // Remitente
        public int UserRemitenteId { get; set; }

        [ForeignKey(nameof(UserRemitenteId))]
        [JsonIgnore]
        public User? UserRemitente { get; set; }

        // Destinatario
        public int? UserDestinatarioId { get; set; }
        [ForeignKey(nameof(UserDestinatarioId))]
        [JsonIgnore]
        public User? UserDestinatario { get; set; }

        // Grupo
        public int? GrupoId { get; set; }
        [JsonIgnore]
        public Grupo? Grupo { get; set; }

        public string Contenido { get; set; }
        public string? UrlArchivo { get; set; }
        public DateTime FechaEnvio { get; set; } = DateTime.Now;
        public bool Leido { get; set; }
    }
    
}
