using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public interface IMensajeService
    {
        Task<ICollection<Mensaje>> GetMensajes(Usuario userPrincipal);
        Task<ICollection<Mensaje>> GetMensajesFromChat(int ChatId);
        Task<BaseResponseGeneric<Mensaje>> Post(Usuario userPrincipal, DtoMensaje request);
        Task<Mensaje> Delete(Usuario userPrincipal, int MensajeId);
        Task<Mensaje> Put(Usuario userPrincipal, int MensajeId, DtoMensaje request);
    }
}
