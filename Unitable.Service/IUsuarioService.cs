using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitable.Dto.Request;
using Unitable.Entities;

namespace Unitable.Service
{
    public interface IUsuarioService
    {
        Task<Usuario> Signup(DtoUsuario request);
        Task<object> Login(DtoLoginUsuario request);
        Task<List<Usuario>> GetUsuarios(Usuario userPrincipal);
        Task<Usuario> EditUsuario(Usuario userPrincipal, DtoEditUsuario request);
        Task<object> GetPremium(Usuario userPrincipal);
        Task<object> DeleteUsuario(Usuario userPrincipal);
        Task<List<Usuario>> GetFollowedUsuarios(Usuario userPrincipal);
        Task<List<Usuario>> GetOtherUsuarios(Usuario userPrincipal);
        Task<object> FollowUsuario(Usuario userPrincipal, int followedUsuarioId);
    }
}
