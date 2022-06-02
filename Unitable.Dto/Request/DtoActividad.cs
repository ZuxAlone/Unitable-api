using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Dto.Request
{
    public class DtoActividad
    {
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public DateTime HoraIni { get; set; }
        public DateTime HoraFin { get; set; }
    }
}
