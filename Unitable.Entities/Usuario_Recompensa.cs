using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Usuario_Recompensa : EntityBase
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int RecompensaId { get; set; }
        public Recompensa Recompensa { get; set; }
    }
}
