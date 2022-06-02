using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Response;
using Unitable.Dto.Request;
using Unitable.Entities;

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

                // Falta el UsuarioId
                // Falta el TemaId

                // Falta el Usuario
                // Falta el Tema

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

            if (entity == null) return (ActionResult)Results.NotFound();

            _context.Actividades.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("edit/")]
        public async Task<ActionResult> Edit(int actividadId, DtoActividad request)
        {
            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null) return (ActionResult)Results.NotFound();

            TimeSpan dif = request.HoraFin - request.HoraIni;

            actividadFromDb.Nombre = request.Nombre;
            actividadFromDb.Detalle = request.Detalle;
            actividadFromDb.HoraIni = request.HoraIni;
            actividadFromDb.HoraFin = request.HoraFin;
            actividadFromDb.DuracionMin = Math.Round(dif.TotalMinutes, 0);

            _context.Actividades.Update(actividadFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/actividad/{actividadFromDb.Id}");

            return (ActionResult)Results.NoContent();
        }

        [HttpPut("finish/")]
        public async Task<ActionResult> Finish(int actividadId)
        {
            var actividadFromDb = await _context.Actividades.FindAsync(actividadId);

            if (actividadFromDb == null) return (ActionResult)Results.NotFound();

            actividadFromDb.Activa = false;

            _context.Actividades.Update(actividadFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/actividad/{actividadFromDb.Id}");

            return (ActionResult)Results.NoContent();
        }
    }
}
