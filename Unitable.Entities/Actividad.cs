using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Actividad : EntityBase
    {
        [StringLength(30)]
        [Required]
        public string Nombre { get; set; }

        [StringLength(100)]
        [Required]
        public string Detalle { get; set; }

        [Required]
        public DateTime HoraIni { get; set; }

        [Required]
        public DateTime HoraFin { get; set; }

        public double DuracionMin { get; set; }
        public bool Activa { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int TemaId { get; set; }
        public Tema Tema { get; set; }
    }
}
