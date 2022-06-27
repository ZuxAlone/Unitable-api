using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;
using Unitable.Service;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class RecompensaController : ControllerBase
    {
        private readonly IRecompensaService _recompensaService;
        private readonly UnitableDbContext _context;

        public RecompensaController(IRecompensaService recompensaService, UnitableDbContext context)
        {
            _recompensaService = recompensaService;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Recompensa>> Get()
        {
            var userPrincipal = GetUserPrincipal();
            var recompensas = await _recompensaService.Get(userPrincipal);

            return Ok(recompensas);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoRecompensa request)
        {
            var resm = await _recompensaService.Post(request);

            if (resm.Success)
            {
                var entity = (Recompensa)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Recompensa/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int RecompensaId)
        {
            var entity = await _recompensaService.Delete(RecompensaId);

            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPut("{RecompensaId:int}")]
        public async Task<ActionResult> Put(int RecompensaId, DtoRecompensa request)
        {
            var resm = await _recompensaService.Put(RecompensaId, request);

            if (resm.Success)
            {
                var RecompensaFromDb = (Recompensa)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Recompensa/{RecompensaFromDb.Id}");
                return Ok(RecompensaFromDb);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpPost("buy/{recompensaId:int}")]
        [Authorize]
        public async Task<ActionResult<Usuario_Recompensa>> BuyRecompensa(int recompensaId)
        {
            var userPrincipal = GetUserPrincipal();
            var resm = await _recompensaService.BuyRecompensa(userPrincipal, recompensaId);

            if (resm.Success)
            {
                var entity = (Usuario_Recompensa)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Recompensa/buy/{recompensaId}");
                return Ok(entity.Id);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpGet("recompesas")]
        [Authorize]
        public async Task<ActionResult<Recompensa>> GetRecompensasByUsuario()
        {
            var userPrincipal = GetUserPrincipal();
            var usuario_recompesas =await _recompensaService.GetRecompensasByUsuario(userPrincipal);

            return Ok(usuario_recompesas);

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
