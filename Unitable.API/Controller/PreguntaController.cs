using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;
using Unitable.Service;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PreguntaController : ControllerBase
    {
        private readonly IPreguntaService _preguntaService;
        private readonly UnitableDbContext _context;

        public PreguntaController(IPreguntaService preguntaService, UnitableDbContext context)
        {
            _preguntaService = preguntaService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Pregunta>> Get()
        {
            var preguntas = await _preguntaService.Get();

            return Ok(preguntas);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoPregunta request)
        {
            var resm = await _preguntaService.Post(request);

            if (resm.Success)
            {
                var entity = (Pregunta)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Pregunta/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int PreguntaId)
        {
            var entity = await _preguntaService.Delete(PreguntaId);

            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPut("{PreguntaId:int}")]
        public async Task<ActionResult> Put(int PreguntaId, DtoPregunta request)
        {
            var resm = await _preguntaService.Put(PreguntaId, request);

            if (resm.Success)
            {
                var PreguntaFromDb = (Pregunta)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Pregunta/{PreguntaFromDb.Id}");
                return Ok(PreguntaFromDb);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpGet("preguntas/{testId:int}")]
        public async Task<ActionResult<Pregunta>> GetPreguntasByTest(int testId)
        {
            var todas_preguntas_test = await _preguntaService.GetPreguntasByTest(testId);
            return Ok(todas_preguntas_test);
        }
    }
}
