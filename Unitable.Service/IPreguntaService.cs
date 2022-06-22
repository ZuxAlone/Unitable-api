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
    public interface IPreguntaService
    {
        Task<List<Pregunta>> Get();
        Task<BaseResponseGeneric<Pregunta>> Post(DtoPregunta request);
        Task<Pregunta> Delete(int PreguntaId);
        Task<BaseResponseGeneric<Pregunta>> Put(int PreguntaId, DtoPregunta request);
        /*Task<List<Pregunta>> GetPreguntasByTest(int testId);*/
    }
}
