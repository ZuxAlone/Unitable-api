using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Chat : EntityBase
    {
        public string Detalle { get; set; }
        public int CantMensajes { get; set; }
    }
}
