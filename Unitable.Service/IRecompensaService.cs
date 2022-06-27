using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public interface IRecompensaService
    {
        Task<List<Recompensa>> Get(Usuario userPrincipal);
        Task<BaseResponseGeneric<Recompensa>> Post(DtoRecompensa request);
        Task<Recompensa> Delete(int RecompensaId);
        Task<BaseResponseGeneric<Recompensa>> Put(int RecompensaId, DtoRecompensa request);
        Task<BaseResponseGeneric<Usuario_Recompensa>> BuyRecompensa(Usuario userPrincipal, int recompensaId);
        Task<List<Recompensa>> GetRecompensasByUsuario(Usuario userPrincipal);
    }
}
