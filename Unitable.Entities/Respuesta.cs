using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Respuesta : EntityBase
    {
        public string RespuestaText { get; set; }
        public bool IsCorrect { get; set; }

        public int PreguntaId { get; set; }

        public Pregunta Pregunta { get; set; }
    }
}
