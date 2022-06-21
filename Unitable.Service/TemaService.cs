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
    public class TemaService: ITemaService
    {
        private readonly UnitableDbContext _context;

        public TemaService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tema>> Get()
        {
            var temas = await _context.Temas.ToListAsync();

            return temas;
        }

        public async Task<BaseResponseGeneric<Tema>> Post(DtoTema request)
        {
            var TemaNameRepetido = await _context.Temas.Where(us => (us.Nombre == request.Nombre)).ToListAsync();

            var resm = new BaseResponseGeneric<Tema>();

            if (TemaNameRepetido.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe un tema con este nombre");
                return resm;
            }

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

            resm.Success = true;
            resm.Result = entity;

            return resm;
        }

        public async Task<Tema> Delete(int TemaId)
        {
            var entity = await _context.Temas.FindAsync(TemaId);

            if (entity != null) {
                _context.Temas.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }
        }

        public async Task<BaseResponseGeneric<Tema>> Put(int TemaId, DtoTema request)
        {
            var resm = new BaseResponseGeneric<Tema>();

            var TemaFromDb = await _context.Temas.FindAsync(TemaId);

            if (TemaFromDb == null) {
                resm.Success = false;
                resm.Errors.Add("El valor no esta definido");
                return resm;
            } 

            var TemaNameRepetido = await _context.Temas.Where(us => (us.Nombre == request.Nombre && us.Id != TemaId)).ToListAsync();

            if (TemaNameRepetido.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe un tema con este nombre");
                return resm;
            }

            var CursoFromDb = await _context.Cursos.FindAsync(request.CursoId);

            if (CursoFromDb == null) {
                resm.Success = false;
                resm.Errors.Add("El valor del curso no esta definido");
                return resm;
            }

            TemaFromDb.Nombre = request.Nombre;
            TemaFromDb.Contenido = request.Contenido;
            TemaFromDb.CursoId = request.CursoId;
            TemaFromDb.Curso = CursoFromDb;

            _context.Temas.Update(TemaFromDb);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = TemaFromDb;
            return resm;
        }

        public async Task<List<Tema>> GetTemasByCurso(int cursoId)
        {
            var temas_curso = await _context.Temas.Where(us => (us.CursoId == cursoId)).ToListAsync();

            return temas_curso;
        }
    }
}
