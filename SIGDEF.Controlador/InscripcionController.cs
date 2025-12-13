using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.DTOs;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.Evento;
using SIGDEF.Entidades.DTOs.Inscripcion;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscripcionController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public InscripcionController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Inscripcion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripciones()
        {
            try
            {
                var inscripciones = await _context.Inscripciones
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Select(i => new InscripcionDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        IdAtleta = i.IdAtleta,
                        IdEvento = i.EventoPrueba.Evento.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        NombreAtleta = i.Atleta.Persona.Nombre + " " + i.Atleta.Persona.Apellido,
                        NombreEvento = i.EventoPrueba.Evento.Nombre,
                        DetallePrueba = $"{i.EventoPrueba.Distancia} - {i.EventoPrueba.TipoBote} - {i.EventoPrueba.CategoriaEdad} - {i.EventoPrueba.SexoCompetencia}",
                        NombreClub = i.Atleta.Club.Nombre,
                        FechaInicioEvento = i.EventoPrueba.Evento.FechaInicio,
                        FechaFinEvento = i.EventoPrueba.Evento.FechaFin
                    })
                    .ToListAsync();

                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Inscripcion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InscripcionDetailDto>> GetInscripcion(int id)
        {
            try
            {
                var inscripcion = await _context.Inscripciones
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Where(i => i.IdInscripcion == id)
                    .Select(i => new InscripcionDetailDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        IdAtleta = i.IdAtleta,
                        IdEvento = i.EventoPrueba.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        Atleta = new AtletaDto
                        {
                            IdPersona = i.Atleta.IdPersona,
                            IdClub = i.Atleta.IdClub,
                            EstadoPago = i.Atleta.EstadoPago,
                            PerteneceSeleccion = i.Atleta.PerteneceSeleccion,
                            Categoria = i.Atleta.Categoria,
                            BecadoEnard = i.Atleta.BecadoEnard,
                            BecadoSdn = i.Atleta.BecadoSdn,
                            MontoBeca = i.Atleta.MontoBeca,
                            PresentoAptoMedico = i.Atleta.PresentoAptoMedico,
                            FechaAptoMedico = i.Atleta.FechaAptoMedico,
                            NombrePersona = i.Atleta.Persona.Nombre + " " + i.Atleta.Persona.Apellido,
                            NombreClub = i.Atleta.Club.Nombre
                        },
                        Evento = new EventoDto
                        {
                            IdEvento = i.EventoPrueba.Evento.IdEvento,
                            Nombre = i.EventoPrueba.Evento.Nombre,
                            FechaInicio = i.EventoPrueba.Evento.FechaInicio,
                            FechaFin = i.EventoPrueba.Evento.FechaFin,
                            CantidadInscripciones = i.EventoPrueba.Evento.Inscripciones.Count,
                            Estado = GetEstadoEvento(i.EventoPrueba.Evento.FechaInicio, i.EventoPrueba.Evento.FechaFin)
                        }
                    })
                    .FirstOrDefaultAsync();

                if (inscripcion == null)
                {
                    return NotFound($"Inscripción con ID {id} no encontrada");
                }

                return Ok(inscripcion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Inscripcion/atleta/5
        [HttpGet("atleta/{idAtleta}")]
        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripcionesPorAtleta(int idAtleta)
        {
            try
            {
                var inscripciones = await _context.Inscripciones
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Where(i => i.IdAtleta == idAtleta)
                    .Select(i => new InscripcionDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        IdAtleta = i.IdAtleta,
                        IdEvento = i.EventoPrueba.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        NombreAtleta = i.Atleta.Persona.Nombre + " " + i.Atleta.Persona.Apellido,
                        NombreEvento = i.EventoPrueba.Evento.Nombre,
                        DetallePrueba = $"{i.EventoPrueba.Distancia} - {i.EventoPrueba.TipoBote} - {i.EventoPrueba.CategoriaEdad} - {i.EventoPrueba.SexoCompetencia}",
                        NombreClub = i.Atleta.Club.Nombre,
                        FechaInicioEvento = i.EventoPrueba.Evento.FechaInicio,
                        FechaFinEvento = i.EventoPrueba.Evento.FechaFin
                    })
                    .ToListAsync();

                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Inscripcion/evento/5
        [HttpGet("evento/{idEvento}")]
        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripcionesPorEvento(int idEvento)
        {
            try
            {
                var inscripciones = await _context.Inscripciones
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(i => i.Atleta)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Where(i => i.EventoPrueba.IdEvento == idEvento)
                    .Select(i => new InscripcionDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        IdAtleta = i.IdAtleta,
                        IdEvento = i.EventoPrueba.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        NombreAtleta = i.Atleta.Persona.Nombre + " " + i.Atleta.Persona.Apellido,
                        NombreEvento = i.EventoPrueba.Evento.Nombre,
                        DetallePrueba = $"{i.EventoPrueba.Distancia} - {i.EventoPrueba.TipoBote} - {i.EventoPrueba.CategoriaEdad} - {i.EventoPrueba.SexoCompetencia}",
                        NombreClub = i.Atleta.Club.Nombre,
                        FechaInicioEvento = i.EventoPrueba.Evento.FechaInicio,
                        FechaFinEvento = i.EventoPrueba.Evento.FechaFin
                    })
                    .ToListAsync();

                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Inscripcion
        [HttpPost]
        public async Task<ActionResult<InscripcionDto>> PostInscripcion(InscripcionCreateDto inscripcionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que el atleta existe
                var atletaExists = await _context.Atletas.AnyAsync(a => a.IdPersona == inscripcionCreateDto.IdAtleta);
                if (!atletaExists)
                {
                    return BadRequest("El atleta especificado no existe");
                }

                // 🔹 Validar que el evento existe
                // 🔹 Validar que la prueba existe y obtener evento
                var eventoPrueba = await _context.EventoPruebas
                    .Include(ep => ep.Evento)
                    .FirstOrDefaultAsync(ep => ep.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba);

                if (eventoPrueba == null)
                {
                    return BadRequest("La prueba especificada no existe");
                }

                // 🔹 Validar que el evento esté activo o próximo
                if (eventoPrueba.Evento.FechaFin < DateTime.UtcNow)
                {
                    return BadRequest("No se puede inscribir a un evento que ya finalizó");
                }

                // 🔹 Validar que no esté ya inscrito en la misma PRUEBA
                var inscripcionExistente = await _context.Inscripciones
                    .AnyAsync(i => i.IdAtleta == inscripcionCreateDto.IdAtleta &&
                                  i.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba);

                if (inscripcionExistente)
                {
                    return BadRequest("El atleta ya está inscrito en esta prueba");
                }

                var inscripcion = new Inscripcion
                {
                    IdAtleta = inscripcionCreateDto.IdAtleta,
                    IdEventoPrueba = inscripcionCreateDto.IdEventoPrueba,
                    FechaInscripcion = inscripcionCreateDto.FechaInscripcion
                };

                _context.Inscripciones.Add(inscripcion);
                await _context.SaveChangesAsync();

                // 🔹 Cargar datos relacionados para la respuesta
                await _context.Entry(inscripcion)
                    .Reference(i => i.Atleta)
                    .LoadAsync();
                await _context.Entry(inscripcion.Atleta)
                    .Reference(a => a.Persona)
                    .LoadAsync();
                await _context.Entry(inscripcion.Atleta)
                    .Reference(a => a.Club)
                    .LoadAsync();
                await _context.Entry(inscripcion)
                    .Reference(i => i.EventoPrueba)
                    .LoadAsync();
                await _context.Entry(inscripcion.EventoPrueba)
                    .Reference(ep => ep.Evento)
                    .LoadAsync();

                var inscripcionDto = new InscripcionDto
                {
                    IdInscripcion = inscripcion.IdInscripcion,
                    IdAtleta = inscripcion.IdAtleta,
                    IdEvento = inscripcion.EventoPrueba.Evento.IdEvento,
                    IdEventoPrueba = inscripcion.IdEventoPrueba,
                    FechaInscripcion = inscripcion.FechaInscripcion,
                    NombreAtleta = inscripcion.Atleta.Persona.Nombre + " " + inscripcion.Atleta.Persona.Apellido,
                    NombreEvento = inscripcion.EventoPrueba.Evento.Nombre,
                    DetallePrueba = $"{inscripcion.EventoPrueba.Distancia} - {inscripcion.EventoPrueba.TipoBote}",
                    NombreClub = inscripcion.Atleta.Club.Nombre,
                    FechaInicioEvento = inscripcion.EventoPrueba.Evento.FechaInicio,
                    FechaFinEvento = inscripcion.EventoPrueba.Evento.FechaFin
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetInscripcion), new { id = inscripcion.IdInscripcion }, inscripcionDto);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Inscripcion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInscripcion(int id, InscripcionCreateDto inscripcionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var inscripcion = await _context.Inscripciones.FindAsync(id);
                if (inscripcion == null)
                {
                    return NotFound($"Inscripción con ID {id} no encontrada");
                }

                // 🔹 Validar que el atleta existe
                var atletaExists = await _context.Atletas.AnyAsync(a => a.IdPersona == inscripcionCreateDto.IdAtleta);
                if (!atletaExists)
                {
                    return BadRequest("El atleta especificado no existe");
                }

                // 🔹 Validar que el evento existe
                // 🔹 Validar que la prueba existe
                var pruebaExists = await _context.EventoPruebas.AnyAsync(ep => ep.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba);
                if (!pruebaExists)
                {
                    return BadRequest("La prueba especificada no existe");
                }

                // 🔹 Validar que no exista otra inscripción del mismo atleta en la misma prueba (excluyendo la actual)
                var inscripcionExistente = await _context.Inscripciones
                    .AnyAsync(i => i.IdAtleta == inscripcionCreateDto.IdAtleta &&
                                  i.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba &&
                                  i.IdInscripcion != id);

                if (inscripcionExistente)
                {
                    return BadRequest("El atleta ya está inscrito en esta prueba");
                }

                // Actualizar propiedades
                inscripcion.IdAtleta = inscripcionCreateDto.IdAtleta;
                inscripcion.IdEventoPrueba = inscripcionCreateDto.IdEventoPrueba;
                inscripcion.FechaInscripcion = inscripcionCreateDto.FechaInscripcion;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InscripcionExists(id))
                {
                    return NotFound($"Inscripción con ID {id} no existe");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE: api/Inscripcion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInscripcion(int id)
        {
            try
            {
                var inscripcion = await _context.Inscripciones.FindAsync(id);
                if (inscripcion == null)
                {
                    return NotFound($"Inscripción con ID {id} no encontrada");
                }

                _context.Inscripciones.Remove(inscripcion);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar la inscripción: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool InscripcionExists(int id)
        {
            return _context.Inscripciones.Any(e => e.IdInscripcion == id);
        }

        private string GetEstadoEvento(DateTime fechaInicio, DateTime fechaFin)
        {
            var ahora = DateTime.UtcNow;

            if (fechaInicio > ahora)
                return "Próximo";
            else if (fechaInicio <= ahora && fechaFin >= ahora)
                return "Activo";
            else
                return "Finalizado";
        }
    }
}