using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Response;
using Unitable.Dto.Request;
using Unitable.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Unitable.Service;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ActividadController: ControllerBase
    {
        private readonly IActividadService _actividadService;
        private readonly UnitableDbContext _context;

        public ActividadController(IActividadService actividadService, UnitableDbContext context)
        {
            _actividadService = actividadService;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Actividad>> Get()
        {
            var todas_Actividades = await _actividadService.Get();

            return Ok(todas_Actividades);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post(DtoActividad request)
        {
            var userPrincipal = GetUserPrincipal();
            var resm = await _actividadService.Post(userPrincipal, request);

            if (resm.Success)
            {
                var entity = (Actividad)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/actividad/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete("{actividadId:int}")]
        [Authorize]
        public async Task<ActionResult> Delete(int actividadId)
        {
            var entity = await _actividadService.Delete(actividadId);

            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPut("{actividadId:int}")]
        [Authorize]
        public async Task<ActionResult> Put(int actividadId, DtoActividad request)
        {
            var resm = await _actividadService.Put(actividadId, request);

            if (resm.Success)
            {
                var entity = (Actividad)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/actividad/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpPut("finish/{actividadId:int}")]
        [Authorize]
        public async Task<ActionResult> Finish(int actividadId)
        {
            var userPrincipal = GetUserPrincipal();
            var resm = await _actividadService.Finish(userPrincipal, actividadId);

            if (resm.Success)
            {
                var entity = (Test)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/actividad/{actividadId}");
                return Ok(entity.Id);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpGet("actividades")]
        [Authorize]
        public async Task<ActionResult<Actividad>> GetActividadesByUsuario()
        {
            var userPrincipal = GetUserPrincipal();
            var mis_Actividades = await _actividadService.GetActividadesByUsuario(userPrincipal);

            return Ok(mis_Actividades);
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
