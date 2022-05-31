using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Mensaje : EntityBase
    {
        public string MensajeTexto { get; set; }
        public DateTime HoraMensaje { get; set; }

        public int UsuarioId { get; set; }
        public int ChatId { get; set; }

        public Usuario Usuario { get; set; }
        public Chat Chat { get; set; }
    }
}
