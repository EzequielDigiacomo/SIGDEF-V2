using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.Tutor;
using SIGDEF.Entidades.DTOs.AtletaTutor;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtletaTutorController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public AtletaTutorController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/AtletaTutor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtletaTutorDto>>> GetAtletasTutores()
        {
            try
            {
                var atletasTutores = await _context.AtletasTutores
                    .Include(at => at.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(at => at.Atleta)
                        .ThenInclude(a => a.Club)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Persona)
                    .Select(at => new AtletaTutorDto
                    {
                        IdAtleta = at.IdAtleta,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        NombreAtleta = at.Atleta.Persona.Nombre + " " + at.Atleta.Persona.Apellido,
                        NombreTutor = at.Tutor.Persona.Nombre + " " + at.Tutor.Persona.Apellido,
                        NombreClub = at.Atleta.Club.Nombre
                    })
                    .ToListAsync();

                return Ok(atletasTutores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/AtletaTutor/atleta/5
        [HttpGet("atleta/{idAtleta}")]
        public async Task<ActionResult<IEnumerable<AtletaTutorDto>>> GetTutoresPorAtleta(int idAtleta)
        {
            try
            {
                var tutores = await _context.AtletasTutores
                    .Include(at => at.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Persona)
                    .Where(at => at.IdAtleta == idAtleta)
                    .Select(at => new AtletaTutorDto
                    {
                        IdAtleta = at.IdAtleta,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        NombreAtleta = at.Atleta.Persona.Nombre + " " + at.Atleta.Persona.Apellido,
                        NombreTutor = at.Tutor.Persona.Nombre + " " + at.Tutor.Persona.Apellido
                    })
                    .ToListAsync();

                return Ok(tutores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/AtletaTutor/tutor/5
        [HttpGet("tutor/{idTutor}")]
        public async Task<ActionResult<IEnumerable<AtletaTutorDto>>> GetAtletasPorTutor(int idTutor)
        {
            try
            {
                var atletas = await _context.AtletasTutores
                    .Include(at => at.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(at => at.Atleta)
                        .ThenInclude(a => a.Club)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Persona)
                    .Where(at => at.IdTutor == idTutor)
                    .Select(at => new AtletaTutorDto
                    {
                        IdAtleta = at.IdAtleta,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        NombreAtleta = at.Atleta.Persona.Nombre + " " + at.Atleta.Persona.Apellido,
                        NombreTutor = at.Tutor.Persona.Nombre + " " + at.Tutor.Persona.Apellido,
                        NombreClub = at.Atleta.Club.Nombre
                    })
                    .ToListAsync();

                return Ok(atletas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/AtletaTutor/5/10 (idAtleta/idTutor)
        [HttpGet("{idAtleta}/{idTutor}")]
        public async Task<ActionResult<AtletaTutorDetailDto>> GetAtletaTutor(int idAtleta, int idTutor)
        {
            try
            {
                var atletaTutor = await _context.AtletasTutores
                    .Include(at => at.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(at => at.Atleta)
                        .ThenInclude(a => a.Club)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Persona)
                    .Where(at => at.IdAtleta == idAtleta && at.IdTutor == idTutor)
                    .Select(at => new AtletaTutorDetailDto
                    {
                        IdAtleta = at.IdAtleta,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        Atleta = new AtletaDto
                        {
                            IdPersona = at.Atleta.IdPersona,
                            IdClub = at.Atleta.IdClub,
                            EstadoPago = at.Atleta.EstadoPago,
                            PerteneceSeleccion = at.Atleta.PerteneceSeleccion,
                            Categoria = at.Atleta.Categoria,
                            NombrePersona = at.Atleta.Persona.Nombre + " " + at.Atleta.Persona.Apellido,
                            NombreClub = at.Atleta.Club.Nombre
                        },
                        Tutor = new TutorDto
                        {
                            IdPersona = at.Tutor.IdPersona,
                            TipoTutor = at.Tutor.TipoTutor,
                            NombrePersona = at.Tutor.Persona.Nombre + " " + at.Tutor.Persona.Apellido
                        }
                    })
                    .FirstOrDefaultAsync();

                if (atletaTutor == null)
                {
                    return NotFound($"Relación Atleta-Tutor no encontrada");
                }

                return Ok(atletaTutor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/AtletaTutor
        [HttpPost]
        public async Task<ActionResult<AtletaTutorDto>> PostAtletaTutor(AtletaTutorCreateDto atletaTutorCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que el atleta existe
                var atletaExists = await _context.Atletas.AnyAsync(a => a.IdPersona == atletaTutorCreateDto.IdAtleta);
                if (!atletaExists)
                {
                    return BadRequest("El atleta especificado no existe");
                }

                // 🔹 Validar que el tutor existe
                var tutorExists = await _context.Tutores.AnyAsync(t => t.IdPersona == atletaTutorCreateDto.IdTutor);
                if (!tutorExists)
                {
                    return BadRequest("El tutor especificado no existe");
                }

                // 🔹 Validar que no existe ya la relación
                var relationExists = await _context.AtletasTutores
                    .AnyAsync(at => at.IdAtleta == atletaTutorCreateDto.IdAtleta && at.IdTutor == atletaTutorCreateDto.IdTutor);

                if (relationExists)
                {
                    return BadRequest("Esta relación Atleta-Tutor ya existe");
                }

                var atletaTutor = new AtletaTutor
                {
                    IdAtleta = atletaTutorCreateDto.IdAtleta,
                    IdTutor = atletaTutorCreateDto.IdTutor,
                    Parentesco = atletaTutorCreateDto.Parentesco
                };

                _context.AtletasTutores.Add(atletaTutor);
                await _context.SaveChangesAsync();

                // 🔹 Cargar datos relacionados para la respuesta
                await _context.Entry(atletaTutor)
                    .Reference(at => at.Atleta)
                    .LoadAsync();
                await _context.Entry(atletaTutor.Atleta)
                    .Reference(a => a.Persona)
                    .LoadAsync();
                await _context.Entry(atletaTutor.Atleta)
                    .Reference(a => a.Club)
                    .LoadAsync();
                await _context.Entry(atletaTutor)
                    .Reference(at => at.Tutor)
                    .LoadAsync();
                await _context.Entry(atletaTutor.Tutor)
                    .Reference(t => t.Persona)
                    .LoadAsync();

                var atletaTutorDto = new AtletaTutorDto
                {
                    IdAtleta = atletaTutor.IdAtleta,
                    IdTutor = atletaTutor.IdTutor,
                    Parentesco = atletaTutor.Parentesco,
                    NombreAtleta = atletaTutor.Atleta.Persona.Nombre + " " + atletaTutor.Atleta.Persona.Apellido,
                    NombreTutor = atletaTutor.Tutor.Persona.Nombre + " " + atletaTutor.Tutor.Persona.Apellido,
                    NombreClub = atletaTutor.Atleta.Club.Nombre
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetAtletaTutor), new { idAtleta = atletaTutor.IdAtleta, idTutor = atletaTutor.IdTutor }, atletaTutorDto);
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

        // PUT: api/AtletaTutor/5/10
        [HttpPut("{idAtleta}/{idTutor}")]
        public async Task<IActionResult> PutAtletaTutor(int idAtleta, int idTutor, AtletaTutorCreateDto atletaTutorCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (idAtleta != atletaTutorCreateDto.IdAtleta || idTutor != atletaTutorCreateDto.IdTutor)
                {
                    return BadRequest("Los IDs de la URL no coinciden con los IDs del objeto");
                }

                var atletaTutor = await _context.AtletasTutores
                    .FirstOrDefaultAsync(at => at.IdAtleta == idAtleta && at.IdTutor == idTutor);

                if (atletaTutor == null)
                {
                    return NotFound($"Relación Atleta-Tutor no encontrada");
                }

                // Actualizar solo el parentesco (las claves no se pueden modificar)
                atletaTutor.Parentesco = atletaTutorCreateDto.Parentesco;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AtletaTutorExists(idAtleta, idTutor))
                {
                    return NotFound($"Relación Atleta-Tutor no existe");
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

        // DELETE: api/AtletaTutor/5/10
        [HttpDelete("{idAtleta}/{idTutor}")]
        public async Task<IActionResult> DeleteAtletaTutor(int idAtleta, int idTutor)
        {
            try
            {
                var atletaTutor = await _context.AtletasTutores
                    .FirstOrDefaultAsync(at => at.IdAtleta == idAtleta && at.IdTutor == idTutor);

                if (atletaTutor == null)
                {
                    return NotFound($"Relación Atleta-Tutor no encontrada");
                }

                _context.AtletasTutores.Remove(atletaTutor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar la relación: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool AtletaTutorExists(int idAtleta, int idTutor)
        {
            return _context.AtletasTutores.Any(e => e.IdAtleta == idAtleta && e.IdTutor == idTutor);
        }
    }
}