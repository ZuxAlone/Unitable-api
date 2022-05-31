using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Tema : EntityBase
    {
        public string Nombre { get; set; }
        public string Contenido { get; set; }

        public int CursoId { get; set; }

        public Curso Curso { get; set; }
    }
}
