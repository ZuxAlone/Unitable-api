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
    public class RespuestaService : IRespuestaService
    {
        private readonly UnitableDbContext _context;

        public RespuestaService(UnitableDbContext context)
        {
            _context = context;
        }

        public async Task<List<Respuesta>> Get()
        {
            var respuestas = await _context.Respuestas.ToListAsync();

            return respuestas;
        }

        public async Task<BaseResponseGeneric<Respuesta>> Post(DtoRespuesta request)
        {
            var RespuestaRepetida = await _context.Respuestas.Where(us => (us.RespuestaText == request.RespuestaText)).ToListAsync();

            var resm = new BaseResponseGeneric<Respuesta>();

            if (RespuestaRepetida.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe un test con este nombre");
                return resm;
            }

            var entity = new Respuesta
            {
                RespuestaText = request.RespuestaText,
                IsCorrect = request.IsCorrect,
                PreguntaId = request.PreguntaId,
                Pregunta = await _context.Preguntas.FindAsync(request.PreguntaId),

                Status = true
            };

            _context.Respuestas.Add(entity);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = entity;

            return resm;
        }

        public async Task<Respuesta> Delete(int RespuestaId)
        {
            var entity = await _context.Respuestas.FindAsync(RespuestaId);

            if (entity != null)
            {
                _context.Respuestas.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            else
            {
                return null;
            }
        }

        public async Task<BaseResponseGeneric<Respuesta>> Put(int RespuestaId, DtoRespuesta request)
        {
            var resm = new BaseResponseGeneric<Respuesta>();

            var RespuestaFromDb = await _context.Respuestas.FindAsync(RespuestaId);

            if (RespuestaFromDb == null)
            {
                resm.Success = false;
                resm.Errors.Add("El valor no esta definido");
                return resm;
            }

            var RespuestaRepetida = await _context.Respuestas.Where(us => (us.RespuestaText == request.RespuestaText && us.Id != RespuestaId)).ToListAsync();

            if (RespuestaRepetida.Count != 0)
            {
                resm.Success = false;
                resm.Errors.Add("Ya existe una respuesta con este nombre");
                return resm;
            }

            var PreguntaFromDb = await _context.Temas.FindAsync(request.PreguntaId);

            if (PreguntaFromDb == null)
            {
                resm.Success = false;
                resm.Errors.Add("El valor de la respuesta no esta definido");
                return resm;
            }

            RespuestaFromDb.RespuestaText = request.RespuestaText;
            RespuestaFromDb.IsCorrect = request.IsCorrect;
            RespuestaFromDb.PreguntaId = request.PreguntaId;
            RespuestaFromDb.Pregunta = await _context.Preguntas.FindAsync(request.PreguntaId);

            _context.Respuestas.Update(RespuestaFromDb);
            await _context.SaveChangesAsync();

            resm.Success = true;
            resm.Result = RespuestaFromDb;
            return resm;
        }

        /*public async Task<List<Respuesta>> GetRespuestasByPregunta(int preguntaId)
        {
            throw new NotImplementedException();
        }*/
    }
}
