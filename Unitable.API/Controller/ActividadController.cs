using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Response;
using Unitable.Dto.Request;
using Unitable.Entities;
using System.Security.Claims;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ActividadController: ControllerBase
    {
        private readonly UnitableDbContext _context;

        public ActividadController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Actividad>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Actividad>>();

            try
            {
                response.Result = await _context.Actividades.ToListAsync();
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                return response;
            }

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoActividad request)
        {
            TimeSpan dif = request.HoraFin - request.HoraIni;
            var entity = new Actividad
            {
                Nombre = request.Nombre,
                Detalle = request.Detalle,
                HoraIni = request.HoraIni,
                HoraFin = request.HoraFin,

                DuracionMin = Math.Round(dif.TotalMinutes, 0),
                Activa = true,

                UsuarioId = request.UsuarioId,
                TemaId = request.TemaId,

                Usuario = await _context.Usuarios.FindAsync(request.UsuarioId),
                Tema = await _context.Temas.FindAsync(request.TemaId),

                Status = true
            };

            _context.Actividades.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/actividad/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int actividadId)
        {
            var entity = await _context.Actividades.FindAsync(actividadId);

            if (entity == null) return NotFound();

            _context.Actividades.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{actividadId:int}")]
        public async Task<ActionResult> Put(int actividadId, DtoActividad request)
        {
            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null) return NotFound();

            TimeSpan dif = request.HoraFin - request.HoraIni;

            actividadFromDb.Nombre = request.Nombre;
            actividadFromDb.Detalle = request.Detalle;
            actividadFromDb.HoraIni = request.HoraIni;
            actividadFromDb.HoraFin = request.HoraFin;
            actividadFromDb.DuracionMin = Math.Round(dif.TotalMinutes, 0);

            _context.Actividades.Update(actividadFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/actividad/{actividadFromDb.Id}");

            return Ok(new { Id = actividadId });
        }

        [HttpPut("finish/{actividadId:int}")]
        public async Task<ActionResult> Finish(int actividadId)
        {
            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null) return NotFound();

            actividadFromDb.Activa = false;

            _context.Actividades.Update(actividadFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/actividad/{actividadFromDb.Id}");

            var tests =  await _context.Tests.Where(us => (us.TemaId == actividadFromDb.TemaId)).ToListAsync();

            Random randomm = new Random();

            int r = randomm.Next(0, tests.Count);

            var test = tests[r];

            return Ok(test);
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
