using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.DTOs;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.AtletaTutor;
using SIGDEF.Entidades.DTOs.Persona;
using SIGDEF.Entidades.DTOs.Tutor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutorController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public TutorController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Tutor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TutorDto>>> GetTutores()
        {
            try
            {
                var tutores = await _context.Tutores
                    .Include(t => t.Persona)
                    .Include(t => t.AtletasTutores)
                    .Select(t => new TutorDto
                    {
                        IdPersona = t.IdPersona,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Persona.Nombre + " " + t.Persona.Apellido,
                        Documento = t.Persona.Documento,
                        Telefono = t.Persona.Telefono,
                        Email = t.Persona.Email,
                        CantidadAtletas = t.AtletasTutores.Count
                    })
                    .ToListAsync();

                return Ok(tutores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Tutor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorDetailDto>> GetTutor(int id)
        {
            try
            {
                var tutor = await _context.Tutores
                    .Include(t => t.Persona)
                    .Include(t => t.AtletasTutores)
                        .ThenInclude(at => at.Atleta)
                        .ThenInclude(a => a.Persona)
                    .Include(t => t.AtletasTutores)
                        .ThenInclude(at => at.Atleta)
                        .ThenInclude(a => a.Club)
                    .Where(t => t.IdPersona == id)
                    .Select(t => new TutorDetailDto
                    {
                        IdPersona = t.IdPersona,
                        TipoTutor = t.TipoTutor,
                        Persona = new PersonaDto
                        {
                            IdPersona = t.Persona.IdPersona,
                            Nombre = t.Persona.Nombre,
                            Apellido = t.Persona.Apellido,
                            Documento = t.Persona.Documento,
                            FechaNacimiento = t.Persona.FechaNacimiento,
                            Email = t.Persona.Email,
                            Telefono = t.Persona.Telefono,
                            Direccion = t.Persona.Direccion
                        },
                        AtletasTutores = t.AtletasTutores.Select(at => new AtletaTutorDto
                        {
                            IdAtleta = at.IdAtleta,
                            IdTutor = at.IdTutor,
                            Parentesco = at.Parentesco,
                            NombreAtleta = at.Atleta.Persona.Nombre + " " + at.Atleta.Persona.Apellido,
                            NombreClub = at.Atleta.Club.Nombre.ToString()
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (tutor == null)
                {
                    return NotFound($"Tutor con ID {id} no encontrado");
                }

                return Ok(tutor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Tutor/tipo/Padre
        [HttpGet("tipo/{tipoTutor}")]
        public async Task<ActionResult<IEnumerable<TutorDto>>> GetTutoresPorTipo(string tipoTutor)
        {
            try
            {
                var tutores = await _context.Tutores
                    .Include(t => t.Persona)
                    .Include(t => t.AtletasTutores)
                    .Where(t => t.TipoTutor.ToLower() == tipoTutor.ToLower())
                    .Select(t => new TutorDto
                    {
                        IdPersona = t.IdPersona,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Persona.Nombre + " " + t.Persona.Apellido,
                        Documento = t.Persona.Documento,
                        Telefono = t.Persona.Telefono,
                        Email = t.Persona.Email,
                        CantidadAtletas = t.AtletasTutores.Count
                    })
                    .ToListAsync();

                return Ok(tutores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Tutor/sinatletas
        [HttpGet("sinatletas")]
        public async Task<ActionResult<IEnumerable<TutorDto>>> GetTutoresSinAtletas()
        {
            try
            {
                var tutores = await _context.Tutores
                    .Include(t => t.Persona)
                    .Include(t => t.AtletasTutores)
                    .Where(t => !t.AtletasTutores.Any())
                    .Select(t => new TutorDto
                    {
                        IdPersona = t.IdPersona,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Persona.Nombre + " " + t.Persona.Apellido,
                        Documento = t.Persona.Documento,
                        Telefono = t.Persona.Telefono,
                        Email = t.Persona.Email,
                        CantidadAtletas = 0
                    })
                    .ToListAsync();

                return Ok(tutores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Tutor
        [HttpPost]
        public async Task<ActionResult<TutorDto>> PostTutor(TutorCreateDto tutorCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que la persona existe
                var personaExists = await _context.Personas.AnyAsync(p => p.IdPersona == tutorCreateDto.IdPersona);
                if (!personaExists)
                {
                    return BadRequest("La persona especificada no existe");
                }

                // 🔹 Validar que no sea tutor ya
                var tutorExists = await _context.Tutores.AnyAsync(t => t.IdPersona == tutorCreateDto.IdPersona);
                if (tutorExists)
                {
                    return BadRequest("Esta persona ya está registrada como tutor");
                }

                // 🔹 Validar que la persona no tenga otro rol (Atleta, Entrenador, etc.)
                var tieneOtroRol = await _context.Atletas.AnyAsync(a => a.IdPersona == tutorCreateDto.IdPersona) ||
                                  await _context.Entrenadores.AnyAsync(e => e.IdPersona == tutorCreateDto.IdPersona) ||
                                  await _context.DelegadosClub.AnyAsync(d => d.IdPersona == tutorCreateDto.IdPersona);

                if (tieneOtroRol)
                {
                    return BadRequest("Esta persona ya tiene otro rol asignado (Atleta, Entrenador o Delegado)");
                }

                var tutor = new Tutor
                {
                    IdPersona = tutorCreateDto.IdPersona,
                    TipoTutor = tutorCreateDto.TipoTutor
                };

                _context.Tutores.Add(tutor);
                await _context.SaveChangesAsync();

                // 🔹 Cargar datos relacionados para la respuesta
                await _context.Entry(tutor)
                    .Reference(t => t.Persona)
                    .LoadAsync();

                var tutorDto = new TutorDto
                {
                    IdPersona = tutor.IdPersona,
                    TipoTutor = tutor.TipoTutor,
                    NombrePersona = tutor.Persona.Nombre + " " + tutor.Persona.Apellido,
                    Documento = tutor.Persona.Documento,
                    Telefono = tutor.Persona.Telefono,
                    Email = tutor.Persona.Email,
                    CantidadAtletas = 0
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetTutor), new { id = tutor.IdPersona }, tutorDto);
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

        // PUT: api/Tutor/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTutor(int id, TutorCreateDto tutorCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != tutorCreateDto.IdPersona)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del tutor");
                }

                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor == null)
                {
                    return NotFound($"Tutor con ID {id} no encontrado");
                }

                // Actualizar propiedades (el IdPersona no se puede modificar ya que es la clave)
                tutor.TipoTutor = tutorCreateDto.TipoTutor;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorExists(id))
                {
                    return NotFound($"Tutor con ID {id} no existe");
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

        // DELETE: api/Tutor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutor(int id)
        {
            try
            {
                var tutor = await _context.Tutores
                    .Include(t => t.AtletasTutores)
                    .FirstOrDefaultAsync(t => t.IdPersona == id);

                if (tutor == null)
                {
                    return NotFound($"Tutor con ID {id} no encontrado");
                }

                // 🔹 Validar que no tenga atletas asociados
                if (tutor.AtletasTutores.Any())
                {
                    return BadRequest("No se puede eliminar el tutor porque tiene atletas asociados");
                }

                _context.Tutores.Remove(tutor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar el tutor: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Tutor/search?term=busqueda
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TutorDto>>> SearchTutores([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest("El término de búsqueda es requerido");
                }

                var tutores = await _context.Tutores
                    .Include(t => t.Persona)
                    .Include(t => t.AtletasTutores)
                    .Where(t => t.Persona.Nombre.Contains(term) ||
                               t.Persona.Apellido.Contains(term) ||
                               t.Persona.Documento.Contains(term) ||
                               t.TipoTutor.Contains(term))
                    .Select(t => new TutorDto
                    {
                        IdPersona = t.IdPersona,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Persona.Nombre + " " + t.Persona.Apellido,
                        Documento = t.Persona.Documento,
                        Telefono = t.Persona.Telefono,
                        Email = t.Persona.Email,
                        CantidadAtletas = t.AtletasTutores.Count
                    })
                    .ToListAsync();

                return Ok(tutores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Tutor/tipos
        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<string>>> GetTiposTutor()
        {
            try
            {
                var tipos = await _context.Tutores
                    .Select(t => t.TipoTutor)
                    .Distinct()
                    .ToListAsync();

                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool TutorExists(int id)
        {
            return _context.Tutores.Any(e => e.IdPersona == id);
        }
    }
}