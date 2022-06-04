using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unitable.API.Service;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Entities;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UnitableDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public UsuarioController(UnitableDbContext context, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [HttpPost("signup/")]
        public async Task<ActionResult<Usuario>> Signup(DtoUsuario request)
        {
            var dbEntry = await _context.Usuarios.FirstOrDefaultAsync(user => user.Correo == request.Correo);
            if (dbEntry != null) return Problem("El correo está en uso");

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

            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/usuario/{entity.Id}");

            return Ok(entity);
        }

        [HttpPost("login/")]
        public async Task<ActionResult> Login(DtoLoginUsuario request)
        {
            var usuario =  await _context.Usuarios.FirstOrDefaultAsync(user => (
                user.Correo == request.Correo && user.Password == request.Password
            ));

            if (usuario == null) return NotFound();

            var generatedToken = _tokenService.BuildToken(_configuration["Jwt:Key"].ToString(), _configuration["Jwt:Issuer"].ToString(), request);
            return Ok(new
            {
                token = generatedToken,
                usuario = usuario
            });
        }

        [HttpGet("/current")]
        [Authorize]
        public async Task<ActionResult<Usuario>> GetCurrentUsuario()
        {
            var usuario = GetUserPrincipal();
            return Ok(usuario);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IObservable<Usuario>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private Usuario GetUserPrincipal()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var correo = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var usuario = _context.Usuarios.FirstOrDefault(user => user.Correo == correo);
            return usuario;
        }
    }
}
