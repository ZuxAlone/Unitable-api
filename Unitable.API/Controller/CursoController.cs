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
    public class CursoController : ControllerBase
    {
        private readonly ICursoService _cursoService;
        private readonly UnitableDbContext _context;

        public CursoController(ICursoService cursoService, UnitableDbContext context)
        {
            _cursoService = cursoService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Curso>> Get()
        {
            var cursos = await _cursoService.Get();

            return Ok(cursos);
        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoCurso request)
        {
            var resm = await _cursoService.Post(request);

            if (resm.Success)
            {
                var entity = (Curso)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Curso/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int CursoId)
        {
            var entity = await _cursoService.Delete(CursoId);

            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPut("{CursoId:int}")]
        public async Task<ActionResult> Put(int CursoId, DtoCurso request)
        {
            var resm = await _cursoService.Put(CursoId, request);

            if (resm.Success)
            {
                var CursoFromDb = (Curso)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Curso/{CursoFromDb.Id}");
                return Ok(CursoFromDb);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }
    }
}
