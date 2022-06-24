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
    public class RespuestaController : ControllerBase
    {
        private readonly IRespuestaService _respuestaService;
        private readonly UnitableDbContext _context;

        public RespuestaController(IRespuestaService respuestaService, UnitableDbContext context)
        {
            _respuestaService = respuestaService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Respuesta>> Get()
        {
            var respuestas = await _respuestaService.Get();

            return Ok(respuestas);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoRespuesta request)
        {
            var resm = await _respuestaService.Post(request);

            if (resm.Success)
            {
                var entity = (Respuesta)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Respuesta/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int RespuestaId)
        {
            var entity = await _respuestaService.Delete(RespuestaId);

            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPut("{RespuestaId:int}")]
        public async Task<ActionResult> Put(int RespuestaId, DtoRespuesta request)
        {
            var resm = await _respuestaService.Put(RespuestaId, request);

            if (resm.Success)
            {
                var RespuestaFromDb = (Respuesta)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Respuesta/{RespuestaFromDb.Id}");
                return Ok(RespuestaFromDb);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpGet("respuestas/{preguntaId:int}")]
        public async Task<ActionResult> GetRespuestasByPregunta(int preguntaId)
        {
            var todas_respuestas_pregunta = await _respuestaService.GetRespuestasByPregunta(preguntaId);
            return Ok(todas_respuestas_pregunta);
        }
    }
}
