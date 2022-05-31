using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Recompensa : EntityBase
    {
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public int PrecioMon { get; set; }
    }
}
