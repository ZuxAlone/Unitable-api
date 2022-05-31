using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Actividad : EntityBase
    {
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public DateTime HoraIni { get; set; }
        public DateTime HoraFin { get; set; }
        public int DuracionMin { get; set; }
        public bool Activa { get; set; }

        public int UsuarioId { get; set; }
        public int TemaId { get; set; }

        public Usuario Usuario { get; set; }
        public Tema Tema { get; set; }
    }
}
