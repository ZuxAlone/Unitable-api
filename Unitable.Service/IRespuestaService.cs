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
    public interface IRespuestaService
    {
        Task<List<Respuesta>> Get();
        Task<BaseResponseGeneric<Respuesta>> Post(DtoRespuesta request);
        Task<Respuesta> Delete(int RespuestaId);
        Task<BaseResponseGeneric<Respuesta>> Put(int RespuestaId, DtoRespuesta request);
        /*Task<List<Respuesta>> GetRespuestasByPregunta(int preguntaId);*/
    }
}
