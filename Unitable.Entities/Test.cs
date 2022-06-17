using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Test : EntityBase
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public int TemaId { get; set; }
        public Tema Tema { get; set; }
    }
}
