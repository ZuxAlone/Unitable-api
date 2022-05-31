using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Entities
{
    public class Usuario_Follow : EntityBase
    {
        public int UsuarioPrincId { get; set; }
        public int UsuarioFollowId { get; set; }
    }
}
