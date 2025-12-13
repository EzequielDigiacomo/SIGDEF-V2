using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.EventoPrueba;
using SIGDEF.Entidades.Enums;

namespace SIGDEF.Controllers
{
    [ApiController]
    [Route("api/eventos/{idEvento}/pruebas")]
    public class EventoPruebaController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public EventoPruebaController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/eventos/5/pruebas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoPrueba>>> GetEventoPruebas(int idEvento)
        {
            // Verificar que el evento exista
            var eventoExiste = await _context.Eventos.AnyAsync(e => e.IdEvento == idEvento);
            if (!eventoExiste)
            {
                return NotFound(new { message = $"Evento con ID {idEvento} no encontrado" });
            }

            return await _context.EventoPruebas
                .Where(ep => ep.IdEvento == idEvento)
                .ToListAsync();
        }

        // GET: api/eventos/5/pruebas/3
        [HttpGet("{idPrueba}")]
        public async Task<ActionResult<EventoPrueba>> GetEventoPrueba(int idEvento, int idPrueba)
        {
            var eventoPrueba = await _context.EventoPruebas
                .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);

            if (eventoPrueba == null)
            {
                return NotFound(new
                {
                    message = $"Prueba con ID {idPrueba} no encontrada en el evento {idEvento}"
                });
            }

            return eventoPrueba;
        }

        // POST: api/eventos/5/pruebas
        [HttpPost]
        public async Task<ActionResult<EventoPrueba>> PostEventoPrueba(
            int idEvento,
            EventoPruebaCreateDto eventoPruebaDto)
        {
            // Verificar que el evento exista
            var evento = await _context.Eventos.FindAsync(idEvento);
            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {idEvento} no encontrado" });
            }

            // Verificar que no exista ya esa prueba en el evento
            var pruebaExistente = await _context.EventoPruebas
                .AnyAsync(ep => ep.IdEvento == idEvento && 
                                ep.Distancia == eventoPruebaDto.Distancia &&
                                ep.CategoriaEdad == eventoPruebaDto.CategoriaEdad &&
                                ep.SexoCompetencia == eventoPruebaDto.SexoCompetencia &&
                                ep.TipoBote == eventoPruebaDto.TipoBote);

            if (pruebaExistente)
            {
                return BadRequest(new
                {
                    message = $"Esta prueba ya existe en este evento"
                });
            }

            // Crear ENTIDAD desde DTO
            var eventoPrueba = new EventoPrueba
            {
                IdEvento = idEvento, // Usamos el idEvento del parámetro
                Distancia = eventoPruebaDto.Distancia,
                CategoriaEdad = eventoPruebaDto.CategoriaEdad,
                SexoCompetencia = eventoPruebaDto.SexoCompetencia,
                TipoBote = eventoPruebaDto.TipoBote,
                PrecioCategoria = eventoPruebaDto.PrecioCategoria
            };

            _context.EventoPruebas.Add(eventoPrueba);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventoPrueba),
                new { idEvento = idEvento, idPrueba = eventoPrueba.IdEventoPrueba },
                eventoPrueba);
        }

        // PUT: api/eventos/5/pruebas/3
        [HttpPut("{idPrueba}")]
        public async Task<IActionResult> PutEventoPrueba(
            int idEvento,
            int idPrueba,
            EventoPruebaUpdateDto eventoPruebaDto)
        {
            if (idPrueba != eventoPruebaDto.IdEventoPrueba)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID de la prueba" });
            }

            var eventoPrueba = await _context.EventoPruebas
                .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);

            if (eventoPrueba == null)
            {
                return NotFound(new
                {
                    message = $"Prueba con ID {idPrueba} no encontrada en el evento {idEvento}"
                });
            }

            // Actualizar propiedades
            eventoPrueba.Distancia = eventoPruebaDto.Distancia;
            eventoPrueba.CategoriaEdad = eventoPruebaDto.CategoriaEdad;
            eventoPrueba.SexoCompetencia = eventoPruebaDto.SexoCompetencia;
            eventoPrueba.TipoBote = eventoPruebaDto.TipoBote;
            eventoPrueba.PrecioCategoria = eventoPruebaDto.PrecioCategoria;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoPruebaExists(idEvento, idPrueba))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/eventos/5/pruebas/3
        [HttpDelete("{idPrueba}")]
        public async Task<IActionResult> DeleteEventoPrueba(int idEvento, int idPrueba)
        {
            var eventoPrueba = await _context.EventoPruebas
                .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);

            if (eventoPrueba == null)
            {
                return NotFound(new
                {
                    message = $"Prueba con ID {idPrueba} no encontrada en el evento {idEvento}"
                });
            }

            _context.EventoPruebas.Remove(eventoPrueba);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventoPruebaExists(int idEvento, int idPrueba)
        {
            return _context.EventoPruebas
                .Any(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);
        }
    }
}