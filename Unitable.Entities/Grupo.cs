using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Grupo : EntityBase
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int NumUsuarios { get; set; }

        public int TemaId { get; set; }
        public int ChatId { get; set; }

        public Tema Tema { get; set; }
        public Chat Chat { get; set; }
    }
}
