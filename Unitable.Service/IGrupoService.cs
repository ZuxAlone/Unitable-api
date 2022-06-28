using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitable.Dto.Request;
using Unitable.Entities;

namespace Unitable.Service
{
    public interface IGrupoService
    {
        Task<ICollection<Grupo>> Get(Usuario userPrincipal);
        Task<ICollection<Grupo>> GetOthers(Usuario userPrincipal);
        Task<Grupo> GetById(int grupoId);
        Task<Grupo> CanCreate(string request);
        Task<Grupo> Post(Usuario userPrincipal, DtoGrupo request);
        Task<Grupo> Delete(int GrupoId);
        Task<Usuario_Grupo> JoinGrupo(Usuario userPrincipal, int GrupoId);
        Task<Grupo> Put(int GrupoId, DtoGrupo request);
    }
}
