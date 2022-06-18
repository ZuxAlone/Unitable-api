using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Entities;
using Unitable.Service;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UnitableDbContext _context;

        public UsuarioController(IUsuarioService usuarioService, UnitableDbContext context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        [HttpPost("signup/")]
        public async Task<ActionResult<Usuario>> Signup(DtoUsuario request)
        {
            var entity = await _usuarioService.Signup(request);
            if (entity == null) return Problem("El correo está en uso");
            return Ok(entity);
        }

        [HttpPost("login/")]
        public async Task<ActionResult> Login(DtoLoginUsuario request)
        {
            var response = await _usuarioService.Login(request);
            if (response == null) return NotFound();
            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Usuario>> GetCurrentUsuario()
        {
            var usuario = GetUserPrincipal();
            return Ok(usuario);
        }

        [HttpGet("usuarios/")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _usuarioService.GetUsuarios();
            return Ok(usuarios);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Usuario>> EditUsuario(DtoEditUsuario request)
        {
            var userPrincipal = GetUserPrincipal();
            var userUpdated = await _usuarioService.EditUsuario(userPrincipal, request);
            return Ok(userUpdated);
        }

        [HttpPut("premium/")]
        [Authorize]
        public async Task<ActionResult> GetPremium()
        {
            var userPrincipal = GetUserPrincipal();
            var response = await _usuarioService.GetPremium(userPrincipal);
            return Ok(response);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteUsuario()
        {
            var userPrincipal = GetUserPrincipal();
            var response = await _usuarioService.DeleteUsuario(userPrincipal);
            return Ok(response);
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
