using Microsoft.AspNetCore.Mvc;
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
    public interface ITemaService
    {
        Task<List<Tema>> Get();
        Task<BaseResponseGeneric<Tema>> Post(DtoTema request);
        Task<Tema> Delete(int TemaId);
        Task<BaseResponseGeneric<Tema>> Put(int TemaId, DtoTema request);
        Task<List<Tema>> GetTemasByCurso(int cursoId);
    }
}
