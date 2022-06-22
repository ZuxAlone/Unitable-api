using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public class TestService : ITestService
    {
        private readonly UnitableDbContext _context;

        public TestService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<List<Test>> Get()
        {
            var tests = await _context.Tests.ToListAsync();

            return tests;
        }

        public async Task<BaseResponseGeneric<Test>> Post(DtoTest request)
        {

            var TestNameRepetido = await _context.Tests.Where(us => (us.Nombre == request.Nombre)).ToListAsync();

            var resm = new BaseResponseGeneric<Test>();

            if (TestNameRepetido.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe un test con este nombre");
                return resm;
            }

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

            resm.Success = true;
            resm.Result = entity;

            return resm;
        }

        public async Task<Test> Delete(int TestId)
        {
            var entity = await _context.Tests.FindAsync(TestId);

            if (entity != null)
            {
                _context.Tests.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }
        }

        public async  Task<BaseResponseGeneric<Test>> Put(int TestId, DtoTest request)
        {
            var resm = new BaseResponseGeneric<Test>();

            var TestFromDb = await _context.Tests.FindAsync(TestId);

            if (TestFromDb == null)
            {
                resm.Success = false;
                resm.Errors.Add("El valor no esta definido");
                return resm;
            }

            var TemaNameRepetido = await _context.Tests.Where(us => (us.Nombre == request.Nombre && us.Id != TestId)).ToListAsync();

            if (TemaNameRepetido.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe un test con este nombre");
                return resm;
            }

            var TemaFromDb = await _context.Temas.FindAsync(request.TemaId);

            if (TemaFromDb == null)
            {
                resm.Success = false;
                resm.Errors.Add("El valor del tema no esta definido");
                return resm;
            }

            TestFromDb.Nombre = request.Nombre;
            TestFromDb.Descripcion = request.Descripcion;
            TestFromDb.TemaId = request.TemaId;
            TestFromDb.Tema = TemaFromDb;

            _context.Tests.Update(TestFromDb);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = TestFromDb;
            return resm;
        }

        /*public async Task<List<Boolean>> TestResultado(List<Boolean> request)
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
        }*/

        /*public async Task<List<Test>> GetTestById(int testId)
        {
            Test test = await _context.Tests.FindAsync(testId);

            return test;
        }*/
    }
}
