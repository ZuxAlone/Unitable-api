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
    public interface ICursoService
    {
        Task<List<Curso>> Get();
        Task<BaseResponseGeneric<Curso>> Post(DtoCurso request);
        Task<Curso> Delete(int CursoId);
        Task<BaseResponseGeneric<Curso>> Put(int CursoId, DtoCurso request);
    }
}
