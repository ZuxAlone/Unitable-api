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
    public class TemaController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public TemaController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Tema>> Get()
        {
            var temas = await _context.Temas.ToListAsync();

            return Ok(temas);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoTema request)
        {
            var entity = new Tema
            {
                Nombre = request.Nombre,
                Contenido = request.Contenido,
                CursoId = request.CursoId,
                Curso = await _context.Cursos.FindAsync(request.CursoId),

                Status = true
            };

            _context.Temas.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Tema/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int TemaId)
        {
            var entity = await _context.Temas.FindAsync(TemaId);

            if (entity == null) return NotFound();

            _context.Temas.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{temaId:int}")]
        public async Task<ActionResult> Put(int TemaId, DtoTema request)
        {
            var TemaFromDb = await _context.Temas.FindAsync(TemaId);

            if (TemaFromDb == null) return NotFound();

            TemaFromDb.Nombre = request.Nombre;
            TemaFromDb.Contenido = request.Contenido;
            TemaFromDb.CursoId = request.CursoId;
            TemaFromDb.Curso = await _context.Cursos.FindAsync(request.CursoId);

            _context.Temas.Update(TemaFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Tema/{TemaFromDb.Id}");

            return Ok(new { Id = TemaId });
        }

        [HttpGet("temas/{cursoId:int}")]
        public async Task<ActionResult<Tema>> GetTemasByCurso(int cursoId)
        {
            var temas_curso = await _context.Temas.Where(us => (us.CursoId == cursoId)).ToListAsync();

            return Ok(temas_curso);
        }
    }
}
