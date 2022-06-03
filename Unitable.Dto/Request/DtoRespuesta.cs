using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Dto.Request
{
    public class DtoRespuesta
    {
        public string RespuestaText { get; set; }
        public bool IsCorrect { get; set; }
        public int PreguntaId { get; set; }
    }
}
