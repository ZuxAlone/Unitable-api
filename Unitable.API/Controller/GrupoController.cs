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
    public class GrupoController : ControllerBase
    {
        private readonly IGrupoService _grupoService;
        private readonly UnitableDbContext _context;

        public GrupoController(IGrupoService grupoService, UnitableDbContext context)
        {
            _grupoService = grupoService;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<Grupo>>> Get()
        {
            var userPrincipal = GetUserPrincipal();
            var grupo = await _grupoService.Get(userPrincipal);
            return Ok(grupo);
        }

        [HttpGet("others")]
        [Authorize]
        public async Task<ActionResult<ICollection<Grupo>>> GetOthers()
        {
            var userPrincipal = GetUserPrincipal();
            var grupo = await _grupoService.GetOthers(userPrincipal);
            return Ok(grupo);
        }

        [HttpGet("{GrupoId:int}")]
        [Authorize]
        public async Task<ActionResult<ICollection<Grupo>>> GetById(int GrupoId)
        {
            var grupo = await _grupoService.GetById(GrupoId);
            if (grupo == null)
                return NotFound("Grupo#" + GrupoId + " Not found");
            return Ok(grupo);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post(DtoGrupo request)
        {
            var userPrincipal = GetUserPrincipal();
            var entity = await _grupoService.Post(userPrincipal, request);

            HttpContext.Response.Headers.Add("location", $"/api/Grupo/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete(int GrupoId)
        {
            var entity = await _grupoService.Delete(GrupoId);
            if (entity == null)
                return NotFound();
            return Ok(entity);
        }

        [HttpPost("Join/{GrupoId:int}")]
        [Authorize]
        public async Task<ActionResult> JoinGrupo(int GrupoId)
        {
            var userPrincipal = GetUserPrincipal();
            var usgr = await _grupoService.JoinGrupo(userPrincipal, GrupoId);
            if (usgr != null)
                return NotFound("Grupo #" + GrupoId + " Not Found");
            return Ok(usgr);
        }

        [HttpPut("{GrupoId:int}")]
        [Authorize]
        public async Task<ActionResult> Put(int GrupoId, DtoGrupo request)
        {
            var GrupoFromDb = await _grupoService.Put(GrupoId, request);
            if (GrupoFromDb == null)
                return NotFound("Grupo #" + GrupoId + " Not Found");
            HttpContext.Response.Headers.Add("location", $"/api/Grupo/{GrupoFromDb.Id}");
            return Ok(GrupoFromDb);
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