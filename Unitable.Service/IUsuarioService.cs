﻿using System;
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
        Task<List<Usuario>> GetUsuarios();
        Task<Usuario> EditUsuario(Usuario userPrincipal, DtoEditUsuario request);
        Task<object> GetPremium(Usuario userPrincipal);
        Task<object> DeleteUsuario(Usuario userPrincipal);
    }
}