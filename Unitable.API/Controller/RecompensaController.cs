using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class RecompensaController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public RecompensaController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Recompensa>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Recompensa>>();

            try
            {
                response.Result = await _context.Recompensas.ToListAsync();
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
        public async Task<ActionResult> Post(DtoRecompensa request)
        {
            var entity = new Recompensa
            {
                Nombre = request.Nombre,
                Detalle = request.Detalle,
                PrecioMon = request.PrecioMon,

                Status = true
            };

            _context.Recompensas.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Recompensa/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int RecompensaId)
        {
            var entity = await _context.Recompensas.FindAsync(RecompensaId);

            if (entity == null) return NotFound();

            _context.Recompensas.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{RecompensaId:int}")]
        public async Task<ActionResult> Put(int RecompensaId, DtoRecompensa request)
        {
            var RecompensaFromDb = await _context.Recompensas.FindAsync(RecompensaId);

            if (RecompensaFromDb == null) return NotFound();

            RecompensaFromDb.Nombre = request.Nombre;
            RecompensaFromDb.Detalle = request.Detalle;
            RecompensaFromDb.PrecioMon = request.PrecioMon;

            _context.Recompensas.Update(RecompensaFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Recompensa/{RecompensaFromDb.Id}");

            return Ok(new { Id = RecompensaId });
        }
    }
}
