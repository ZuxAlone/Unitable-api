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

        public async Task<List<Usuario>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
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
    }
}
