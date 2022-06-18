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
    public class CursoController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public CursoController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Curso>> Get()
        {
            var cursos = await _context.Cursos.ToListAsync();

            return Ok(cursos);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoCurso request)
        {
            var entity = new Curso
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,

                Status = true
            };

            _context.Cursos.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Curso/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int CursoId)
        {
            var entity = await _context.Cursos.FindAsync(CursoId);

            if (entity == null) return NotFound();

            _context.Cursos.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{CursoId:int}")]
        public async Task<ActionResult> Put(int CursoId, DtoCurso request)
        {
            var CursoFromDb = await _context.Cursos.FindAsync(CursoId);

            if (CursoFromDb == null) return NotFound();

            CursoFromDb.Nombre = request.Nombre;
            CursoFromDb.Descripcion = request.Descripcion;

            //_context.Entry(CursoFromDb).State = EntityState.Modified;
            _context.Cursos.Update(CursoFromDb);

            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Curso/{CursoFromDb.Id}");

            return Ok(new { Id = CursoId });
        }
    }
}
