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
    public class RespuestaController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public RespuestaController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Respuesta>> Get()
        {
            var respuestas = await _context.Respuestas.ToListAsync();

            return Ok(respuestas);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoRespuesta request)
        {
            var entity = new Respuesta
            {
                RespuestaText = request.RespuestaText,
                IsCorrect = request.IsCorrect,
                PreguntaId = request.PreguntaId,
                Pregunta = await _context.Preguntas.FindAsync(request.PreguntaId),

                Status = true
            };

            _context.Respuestas.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Respuesta/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int RespuestaId)
        {
            var entity = await _context.Respuestas.FindAsync(RespuestaId);

            if (entity == null) return NotFound();

            _context.Respuestas.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{RespuestaId:int}")]
        public async Task<ActionResult> Put(int RespuestaId, DtoRespuesta request)
        {
            var RespuestaFromDb = await _context.Respuestas.FindAsync(RespuestaId);

            if (RespuestaFromDb == null) return NotFound();

            RespuestaFromDb.RespuestaText = request.RespuestaText;
            RespuestaFromDb.IsCorrect = request.IsCorrect;
            RespuestaFromDb.PreguntaId = request.PreguntaId;
            RespuestaFromDb.Pregunta = await _context.Preguntas.FindAsync(request.PreguntaId);

            _context.Respuestas.Update(RespuestaFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Respuesta/{RespuestaFromDb.Id}");

            return Ok(new { Id = RespuestaId });
        }
    }
}
