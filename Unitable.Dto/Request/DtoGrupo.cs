using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Dto.Request
{
    public class DtoGrupo
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string DetalleChat { get; set; }

        public int TemaId { get; set; }
    }
}
