using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Dto.Request
{
    public class DtoMensaje
    {
        public string MensajeTexto { get; set; }
        public int UsuarioId { get; set; }
        public int ChatId { get; set; }
    }
}
