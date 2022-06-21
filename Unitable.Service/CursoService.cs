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
    public class CursoService : ICursoService
    {
        private readonly UnitableDbContext _context;

        public CursoService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<List<Curso>> Get()
        {
            var cursos = await _context.Cursos.ToListAsync();

            return cursos;
        }

        public async Task<BaseResponseGeneric<Curso>> Post(DtoCurso request)
        {
            var CursoNameRepetido = await _context.Cursos.Where(us => (us.Nombre == request.Nombre)).ToListAsync();

            var resm = new BaseResponseGeneric<Curso>();

            if (CursoNameRepetido.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe un curso con este nombre");
                return resm;
            }
               
            var entity = new Curso
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,

                Status = true
            };

            _context.Cursos.Add(entity);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = entity;

            return resm;
        }

        public async Task<Curso> Delete(int CursoId)
        {
            var entity = await _context.Cursos.FindAsync(CursoId);

            if (entity != null)
            {
                _context.Cursos.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else {
                return null;
            }
        }

        public async Task<BaseResponseGeneric<Curso>> Put(int CursoId, DtoCurso request)
        {
            var resm = new BaseResponseGeneric<Curso>();

            var CursoFromDb = await _context.Cursos.FindAsync(CursoId);

            if (CursoFromDb == null) {
                resm.Success = false;
                resm.Errors.Add("El valor no esta definido");
                return resm;
            }

            var CursoNameRepetido = await _context.Cursos.Where(us => (us.Nombre == request.Nombre && us.Id != CursoId)).ToListAsync();

            if (CursoNameRepetido.Count != 0) {
                resm.Success = false;
                resm.Errors.Add("Ya existe un curso con este nombre");
                return resm;
            }

            CursoFromDb.Nombre = request.Nombre;
            CursoFromDb.Descripcion = request.Descripcion;

            _context.Cursos.Update(CursoFromDb);

            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = CursoFromDb;
            return resm;
        }
    }
}
