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
    public class PreguntaController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public PreguntaController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Pregunta>> Get()
        {
            var preguntas = await _context.Preguntas.ToListAsync();

            return Ok(preguntas);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoPregunta request)
        {
            var entity = new Pregunta
            {
                PreguntaText = request.PreguntaText,
                TestId = request.TestId,
                Test = await _context.Tests.FindAsync(request.TestId),

                Status = true
            };

            _context.Preguntas.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Pregunta/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int PreguntaId)
        {
            var entity = await _context.Preguntas.FindAsync(PreguntaId);

            if (entity == null) return NotFound();

            _context.Preguntas.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{PreguntaId:int}")]
        public async Task<ActionResult> Put(int PreguntaId, DtoPregunta request)
        {
            var PreguntaFromDb = await _context.Preguntas.FindAsync(PreguntaId);

            if (PreguntaFromDb == null) return NotFound();

            PreguntaFromDb.PreguntaText = request.PreguntaText;
            PreguntaFromDb.TestId = request.TestId;
            PreguntaFromDb.Test = await _context.Tests.FindAsync(request.TestId);

            _context.Preguntas.Update(PreguntaFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Pregunta/{PreguntaFromDb.Id}");

            return Ok(new { Id = PreguntaId });
        }
    }
}
