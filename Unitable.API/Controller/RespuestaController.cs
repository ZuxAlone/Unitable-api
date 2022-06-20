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

        [HttpGet("respuestas/{preguntaId:int}")]
        public async Task<ActionResult> GetRespuestasByPregunta(int preguntaId)
        {
            var todas_respuestas_pregunta = await _context.Respuestas.Where(us => (us.PreguntaId == preguntaId)).ToListAsync();
            List<Respuesta> respuestas_test = new();
            var correcta = todas_respuestas_pregunta.FindAll(us => us.IsCorrect == true);

            if(correcta.Count == 0)
            {
                return Problem("No hay ninguna respuesta correcta para esta pregunta, ingrese una.");
            }

            respuestas_test.Add(correcta[0]);
            if (todas_respuestas_pregunta.Count > 0)
            {
                if (todas_respuestas_pregunta.Count > 5)
                {
                    Random randomm = new Random();
                    int r = (randomm.Next(0, todas_respuestas_pregunta.Count));
                    var respuesta = todas_respuestas_pregunta[r];

                    while (respuestas_test.Count < 5)
                    {
                        var temp = respuestas_test.FindAll(us => us.Id == respuesta.Id);
                        if (temp.Count == 0)
                        {
                            respuestas_test.Add(respuesta);
                            r = (randomm.Next(0, todas_respuestas_pregunta.Count));
                            respuesta = todas_respuestas_pregunta[r];
                        }
                        else
                        {
                            r = (randomm.Next(0, todas_respuestas_pregunta.Count));
                            respuesta = todas_respuestas_pregunta[r];
                        }
                }
                }
                else
                {
                    respuestas_test = todas_respuestas_pregunta;
                }

                return Ok(respuestas_test);
            }
            else
            {
                return Problem("No hay respuestas para esta pregunta");
            }
        }
    }
}
