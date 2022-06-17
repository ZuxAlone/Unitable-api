using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Pregunta : EntityBase
    {
        public string PreguntaText { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }
    }
}
