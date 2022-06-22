using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;
using Unitable.Service;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly UnitableDbContext _context;

        public TestController(ITestService testService, UnitableDbContext context)
        {
            _testService = testService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Test>>>> Get()
        {
            var tests = await _testService.Get();

            return Ok(tests);

        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoTest request)
        {
            var resm = await _testService.Post(request);

            if (resm.Success)
            {
                var entity = (Test)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Test/{entity.Id}");
                return Ok(entity);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int TestId)
        {
            var entity = await _testService.Delete(TestId);

            if (entity == null) return NotFound();

            return Ok(entity);
        }

        [HttpPut("{TestId:int}")]
        public async Task<ActionResult> Put(int TestId, DtoTest request)
        {
            var resm = await _testService.Put(TestId, request);

            if (resm.Success)
            {
                var TestFromDb = (Test)resm.Result;
                HttpContext.Response.Headers.Add("location", $"/api/Test/{TestFromDb.Id}");
                return Ok(TestFromDb);
            }
            else
            {
                return NotFound(resm.Errors);
            }
        }

        [HttpPost("resultado")]
        [Authorize]
        public async Task<ActionResult<List<Boolean>>> TestResultado(List<Boolean> request)
        {
            int c = 0;
            foreach (Boolean respuesta in request)
            {
                if (respuesta == true) { c++; }
            }

            double percentcorrect = ((double)c / (double)request.Count()) * 100.00;
            var userPrincipal = GetUserPrincipal();

            if (percentcorrect > 75)
            {
                userPrincipal.NumTestAprobados = userPrincipal.NumTestAprobados + 1;
                userPrincipal.NumMonedas = userPrincipal.NumMonedas + 20;
            }

            await _context.SaveChangesAsync();
            return Ok(percentcorrect);

        }

        [HttpGet("test/{testId:int}")]
        public async Task<ActionResult<Test>> GetTestById(int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            return Ok(test);
        }

        private Usuario GetUserPrincipal()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var correo = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var usuario = _context.Usuarios.FirstOrDefault(user => user.Correo == correo);
            return usuario;
        }
    }
}
