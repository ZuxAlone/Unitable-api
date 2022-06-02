using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UsuarioController(UnitableDbContext context)
        {
            _context = context;
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

        [HttpPost("signin/")]
        public async Task<ActionResult<Usuario>> Singin(DtoSigninUsuario request)
        {
            var usuario =  await _context.Usuarios.FirstOrDefaultAsync(user => (
                user.Correo == request.Correo && user.Password == request.Password
            ));

            if (usuario == null) return NotFound();
            return Ok(usuario);
        } 
    }
}
