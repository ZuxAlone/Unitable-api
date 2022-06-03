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
    public class TestController : ControllerBase
    {
        private readonly UnitableDbContext _context;

        public TestController(UnitableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Test>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Test>>();

            try
            {
                response.Result = await _context.Tests.ToListAsync();
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
        public async Task<ActionResult> Post(DtoTest request)
        {
            var entity = new Test
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                TemaId = request.TemaId,
                Tema = await _context.Temas.FindAsync(request.TemaId),

                Status = true
            };

            _context.Tests.Add(entity);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Test/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int TestId)
        {
            var entity = await _context.Tests.FindAsync(TestId);

            if (entity == null) return NotFound();

            _context.Tests.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpPut("{TestId:int}")]
        public async Task<ActionResult> Put(int TestId, DtoTest request)
        {
            var TestFromDb = await _context.Tests.FindAsync(TestId);

            if (TestFromDb == null) return NotFound();

            TestFromDb.Nombre = request.Nombre;
            TestFromDb.Descripcion = request.Descripcion;
            TestFromDb.TemaId = request.TemaId;
            TestFromDb.Tema = await _context.Temas.FindAsync(request.TemaId);

            _context.Tests.Update(TestFromDb);
            await _context.SaveChangesAsync();

            HttpContext.Response.Headers.Add("location", $"/api/Test/{TestFromDb.Id}");

            return Ok(new { Id = TestId });
        }
    }
}
