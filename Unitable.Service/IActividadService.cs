﻿using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public interface IActividadService
    {
        Task<List<Actividad>> Get();
        Task<BaseResponseGeneric<Actividad>> Post(Usuario userPrincipal, DtoActividad request);
        Task<Actividad> Delete(int actividadId);
        Task<BaseResponseGeneric<Actividad>> Put(int actividadId, DtoActividad request);
        Task<BaseResponseGeneric<Test>> Finish(Usuario userPrincipal, int actividadId);
        Task<List<Actividad>> GetActividadesByUsuario(Usuario userPrincipal);
    }
}
