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
    public class TemaController : ControllerBase
    {
        private readonly ITemaService _temaService;
        private readonly UnitableDbContext _context;

        public TemaController(ITemaService temaService ,UnitableDbContext context)
        {
            _temaService = temaService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Tema>> Get()
        {
            var temas = await _temaService.Get();

            return Ok(temas);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoTema request)
        {
            var resm = await _temaService.Post(request);

            if (resm.Success)
            {
                var entity = (Tema)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Tema/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int TemaId)
        {
            var entity = await _temaService.Delete(TemaId);

            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPut("{temaId:int}")]
        public async Task<ActionResult> Put(int temaId, DtoTema request)
        {
            var resm = await _temaService.Put(temaId, request);

            if (resm.Success)
            {
                var TemaFromDb = (Tema)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Tema/{TemaFromDb.Id}");
                return Ok(TemaFromDb);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpGet("temas/{cursoId:int}")]
        public async Task<ActionResult<Tema>> GetTemasByCurso(int cursoId)
        {
            var temas_curso = await _temaService.GetTemasByCurso(cursoId);

            return Ok(temas_curso);
        }
    }
}
