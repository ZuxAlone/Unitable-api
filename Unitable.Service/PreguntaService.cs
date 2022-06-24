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
    public class PreguntaService : IPreguntaService
    {
        private readonly UnitableDbContext _context;

        public PreguntaService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pregunta>> Get()
        {
            var preguntas = await _context.Preguntas.ToListAsync();

            return preguntas;
        }

        public async Task<BaseResponseGeneric<Pregunta>> Post(DtoPregunta request)
        {
            var PreguntaNameRepetida = await _context.Preguntas.Where(us => (us.PreguntaText == request.PreguntaText)).ToListAsync();

            var resm = new BaseResponseGeneric<Pregunta>();

            if (PreguntaNameRepetida.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe un tema con este nombre");
                return resm;
            }

            var entity = new Pregunta
            {
                PreguntaText = request.PreguntaText,
                TestId = request.TestId,
                Test = await _context.Tests.FindAsync(request.TestId),

                Status = true
            };

            _context.Preguntas.Add(entity);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = entity;

            return resm;
        }

        public async Task<Pregunta> Delete(int PreguntaId)
        {
            var entity = await _context.Preguntas.FindAsync(PreguntaId);

            if (entity != null)
            {
                _context.Preguntas.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }
        }

        public async Task<BaseResponseGeneric<Pregunta>> Put(int PreguntaId, DtoPregunta request)
        {
            var resm = new BaseResponseGeneric<Pregunta>();

            var PreguntaFromDb = await _context.Preguntas.FindAsync(PreguntaId);

            if (PreguntaFromDb == null)
            {
                resm.Success = false;
                resm.Errors.Add("El valor no esta definido");
                return resm;
            }

            var PreguntaRepetida = await _context.Preguntas.Where(us => (us.PreguntaText == request.PreguntaText && us.Id != PreguntaId)).ToListAsync();

            if (PreguntaRepetida.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe esta pregunta");
                return resm;
            }

            var TestFromDb = await _context.Tests.FindAsync(request.TestId);

            if (TestFromDb == null)
            {
                resm.Success = false;
                resm.Errors.Add("El valor del test no esta definido");
                return resm;
            }

            PreguntaFromDb.PreguntaText = request.PreguntaText;
            PreguntaFromDb.TestId = request.TestId;
            PreguntaFromDb.Test = await _context.Tests.FindAsync(request.TestId);

            _context.Preguntas.Update(PreguntaFromDb);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = PreguntaFromDb;
            return resm;
        }

        public async Task<List<Pregunta>> GetPreguntasByTest(int testId)
        {
            var todas_preguntas_test = await _context.Preguntas.Where(us => (us.TestId == testId)).ToListAsync();
            List<Pregunta> preguntas_test = new();

            if (todas_preguntas_test.Count > 0)
            {
                if (todas_preguntas_test.Count > 5)
                {
                    Random randomm = new Random();
                    int r = (randomm.Next(0, todas_preguntas_test.Count));
                    var pregunta = todas_preguntas_test[r];

                    while (preguntas_test.Count < 5)
                    {
                        var temp = preguntas_test.FindAll(us => us.Id == pregunta.Id);
                        if (temp.Count == 0)
                        {
                            preguntas_test.Add(pregunta);
                            r = (randomm.Next(0, todas_preguntas_test.Count));
                            pregunta = todas_preguntas_test[r];
                        }
                        else
                        {
                            r = (randomm.Next(0, todas_preguntas_test.Count));
                            pregunta = todas_preguntas_test[r];
                        }
                    }
                }
                else
                {
                    preguntas_test = todas_preguntas_test;
                }

                return preguntas_test;
            }
            else
            {
                return null;
            }
        }
    }
}
