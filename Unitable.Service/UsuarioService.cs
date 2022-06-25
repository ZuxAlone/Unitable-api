using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Entities;

namespace Unitable.Service
{
    public class UsuarioService : IUsuarioService
    {

        private readonly UnitableDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public UsuarioService(UnitableDbContext context, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<Usuario> Signup(DtoUsuario request)
        {
            var dbEntry = await _context.Usuarios.FirstOrDefaultAsync(user => user.Correo == request.Correo);
            if (dbEntry != null) return null;

            var entity = new Usuario
            {
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                Correo = request.Correo,
                Password = request.Password,
                Carrera = request.Carrera,
                Tipo = request.Tipo,

                NumActCompletas = 0,
                NumTestAprobados = 0,
                NumMonedas = 0,
                IsPremium = false,

                Status = true
            };

            await _context.Usuarios.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<object> Login(DtoLoginUsuario request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(user => (
                user.Correo == request.Correo && user.Password == request.Password
            ));

            if (usuario == null) return null;

            var generatedToken = _tokenService.BuildToken(_configuration["Jwt:Key"].ToString(), _configuration["Jwt:Issuer"].ToString(), request);

            return new
            {
                usuario = usuario,
                token = generatedToken
            };
        }

        public async Task<List<Usuario>> GetUsuarios(Usuario userPrincipal)
        {
            var usuarios = await _context.Usuarios.Where(us => us.Id != userPrincipal.Id).ToListAsync();
            return usuarios;
        }

        public async Task<Usuario> EditUsuario(Usuario userPrincipal, DtoEditUsuario request)
        {
            userPrincipal.Nombres = request.Nombres;
            userPrincipal.Apellidos = request.Apellidos;
            userPrincipal.Carrera = request.Carrera;
            userPrincipal.Tipo = request.Tipo;

            _context.Usuarios.Update(userPrincipal);
            await _context.SaveChangesAsync();
            return userPrincipal;
        }

        public async Task<object> GetPremium(Usuario userPrincipal)
        {
            if (userPrincipal.IsPremium) userPrincipal.IsPremium = false;
            else userPrincipal.IsPremium = true;

            _context.Usuarios.Update(userPrincipal);
            await _context.SaveChangesAsync();

            return new
            {
                premium = userPrincipal.IsPremium,
                mensaje = "Has cambiado tu suscripción!"
            };
        }

        public async Task<object> DeleteUsuario(Usuario userPrincipal)
        {
            _context.Usuarios.Remove(userPrincipal);
            await _context.SaveChangesAsync();
            return new
            {
                usuario = userPrincipal,
                mensaje = "Usuario Eliminado"
            };
        }

        public async Task<List<Usuario>> GetFollowedUsuarios(Usuario userPrincipal)
        {
            List<Usuario> followedUsuarios = new List<Usuario>();
            var usuario_follows = await _context.Usuario_Follows.Where(us_fo => us_fo.UsuarioPrincId == userPrincipal.Id).ToListAsync();
            foreach (var usuario_follow in usuario_follows)
            {
                var usuario = await _context.Usuarios.FindAsync(usuario_follow.UsuarioFollowId);
                followedUsuarios.Add(usuario);
            }
            return followedUsuarios;
        }

        public async Task<List<Usuario>> GetOtherUsuarios(Usuario userPrincipal)
        {
            var usuarios = await _context.Usuarios.Where(us => us.Id != userPrincipal.Id).ToListAsync();
            var usuario_follows = await _context.Usuario_Follows.Where(us_fo => us_fo.UsuarioPrincId == userPrincipal.Id).ToListAsync();
            foreach (var usuario_follow in usuario_follows)
            {
                var usuario = await _context.Usuarios.FindAsync(usuario_follow.UsuarioFollowId);
                usuarios.Remove(usuario);
            }
            return usuarios;
        }

        public async Task<object> FollowUsuario(Usuario userPrincipal, int followedUsuarioId)
        {
            var usuario_Follow = await _context.Usuario_Follows.FirstOrDefaultAsync(us_fo => (
                us_fo.UsuarioPrincId == userPrincipal.Id && us_fo.UsuarioFollowId == followedUsuarioId
            ));

            if (usuario_Follow == null)
            {
                var usuario_follow_entity = new Usuario_Follow
                {
                    UsuarioPrincId = userPrincipal.Id,
                    UsuarioFollowId = followedUsuarioId
                };

                await _context.Usuario_Follows.AddAsync(usuario_follow_entity);
                await _context.SaveChangesAsync();
                return new
                {
                    usuario_follow = usuario_follow_entity,
                    mensaje = "Has seguido a un usuario."
                };
            }

            _context.Usuario_Follows.Remove(usuario_Follow);
            await _context.SaveChangesAsync();
            return new
            {
                usuario_unfollow = usuario_Follow,
                mensaje = "Dejaste de seguir a un usuario."
            };
        }
    }
}
