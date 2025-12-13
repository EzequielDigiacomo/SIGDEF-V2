using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.DTOs;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.DTOs.Entrenador;
using SIGDEF.Entidades.DTOs.Persona;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntrenadorController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public EntrenadorController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Entrenador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadores()
        {
            try
            {
                var entrenadores = await _context.Entrenadores
                    .Include(e => e.Persona)
                    .Include(e => e.Club)
                    .Select(e => new EntrenadorDto
                    {
                        IdPersona = e.IdPersona,
                        IdClub = e.IdClub,
                        Licencia = e.Licencia,
                        PerteneceSeleccion = e.PerteneceSeleccion,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard,
                        BecadoSdn = e.BecadoSdn,
                        MontoBeca = e.MontoBeca,
                        PresentoAptoMedico = e.PresentoAptoMedico,
                        NombrePersona = e.Persona.Nombre + " " + e.Persona.Apellido,
                        NombreClub = e.Club.Nombre,
                        SiglasClub = e.Club.Siglas
                    })
                    .ToListAsync();

                return Ok(entrenadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Entrenador/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntrenadorDetailDto>> GetEntrenador(int id)
        {
            try
            {
                var entrenador = await _context.Entrenadores
                    .Include(e => e.Persona)
                    .Include(e => e.Club)
                    .Where(e => e.IdPersona == id)
                    .Select(e => new EntrenadorDetailDto
                    {
                        IdPersona = e.IdPersona,
                        IdClub = e.IdClub,
                        Licencia = e.Licencia,
                        PerteneceSeleccion = e.PerteneceSeleccion,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard,
                        BecadoSdn = e.BecadoSdn,
                        MontoBeca = e.MontoBeca,
                        PresentoAptoMedico = e.PresentoAptoMedico,
                        Persona = new PersonaDto
                        {
                            IdPersona = e.Persona.IdPersona,
                            Nombre = e.Persona.Nombre,
                            Apellido = e.Persona.Apellido,
                            Documento = e.Persona.Documento,
                            FechaNacimiento = e.Persona.FechaNacimiento,
                            Email = e.Persona.Email,
                            Telefono = e.Persona.Telefono,
                            Direccion = e.Persona.Direccion
                        },
                        Club = new ClubDto
                        {
                            IdClub = e.Club.IdClub,
                            Nombre = e.Club.Nombre,
                            Direccion = e.Club.Direccion,
                            Telefono = e.Club.Telefono,
                            Siglas = e.Club.Siglas
                        }
                    })
                    .FirstOrDefaultAsync();

                if (entrenador == null)
                {
                    return NotFound($"Entrenador con ID {id} no encontrado");
                }

                return Ok(entrenador);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Entrenador/club/5
        [HttpGet("club/{idClub}")]
        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresPorClub(int idClub)
        {
            try
            {
                var entrenadores = await _context.Entrenadores
                    .Include(e => e.Persona)
                    .Include(e => e.Club)
                    .Where(e => e.IdClub == idClub)
                    .Select(e => new EntrenadorDto
                    {
                        IdPersona = e.IdPersona,
                        IdClub = e.IdClub,
                        Licencia = e.Licencia,
                        PerteneceSeleccion = e.PerteneceSeleccion,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard,
                        BecadoSdn = e.BecadoSdn,
                        MontoBeca = e.MontoBeca,
                        PresentoAptoMedico = e.PresentoAptoMedico,
                        NombrePersona = e.Persona.Nombre + " " + e.Persona.Apellido,
                        NombreClub = e.Club.Nombre,
                        SiglasClub = e.Club.Siglas
                    })
                    .ToListAsync();

                return Ok(entrenadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Entrenador
        [HttpPost]
        public async Task<ActionResult<EntrenadorDto>> PostEntrenador(EntrenadorCreateDto entrenadorCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que la persona existe
                var personaExists = await _context.Personas.AnyAsync(p => p.IdPersona == entrenadorCreateDto.IdPersona);
                if (!personaExists)
                {
                    return BadRequest("La persona especificada no existe");
                }

                // 🔹 Validar que el club existe
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == entrenadorCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // 🔹 Validar que no sea entrenador ya
                var entrenadorExists = await _context.Entrenadores.AnyAsync(e => e.IdPersona == entrenadorCreateDto.IdPersona);
                if (entrenadorExists)
                {
                    return BadRequest("Esta persona ya está registrada como entrenador");
                }

                var entrenador = new Entrenador
                {
                    IdPersona = entrenadorCreateDto.IdPersona,
                    IdClub = entrenadorCreateDto.IdClub,
                    Licencia = entrenadorCreateDto.Licencia,
                    PerteneceSeleccion = entrenadorCreateDto.PerteneceSeleccion,
                    CategoriaSeleccion = entrenadorCreateDto.CategoriaSeleccion,
                    BecadoEnard = entrenadorCreateDto.BecadoEnard,
                    BecadoSdn = entrenadorCreateDto.BecadoSdn,
                    MontoBeca = entrenadorCreateDto.MontoBeca,
                    PresentoAptoMedico = entrenadorCreateDto.PresentoAptoMedico
                };

                _context.Entrenadores.Add(entrenador);
                await _context.SaveChangesAsync();

                // 🔹 Cargar datos relacionados para la respuesta
                await _context.Entry(entrenador)
                    .Reference(e => e.Persona)
                    .LoadAsync();
                await _context.Entry(entrenador)
                    .Reference(e => e.Club)
                    .LoadAsync();

                var entrenadorDto = new EntrenadorDto
                {
                    IdPersona = entrenador.IdPersona,
                    IdClub = entrenador.IdClub,
                    Licencia = entrenador.Licencia,
                    PerteneceSeleccion = entrenador.PerteneceSeleccion,
                    CategoriaSeleccion = entrenador.CategoriaSeleccion,
                    BecadoEnard = entrenador.BecadoEnard,
                    BecadoSdn = entrenador.BecadoSdn,
                    MontoBeca = entrenador.MontoBeca,
                    PresentoAptoMedico = entrenador.PresentoAptoMedico,
                    NombrePersona = entrenador.Persona.Nombre + " " + entrenador.Persona.Apellido,
                    NombreClub = entrenador.Club.Nombre,
                    SiglasClub = entrenador.Club.Siglas
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetEntrenador), new { id = entrenador.IdPersona }, entrenadorDto);
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

        // PUT: api/Entrenador/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntrenador(int id, EntrenadorCreateDto entrenadorCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != entrenadorCreateDto.IdPersona)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del entrenador");
                }

                var entrenador = await _context.Entrenadores.FindAsync(id);
                if (entrenador == null)
                {
                    return NotFound($"Entrenador con ID {id} no encontrado");
                }

                // 🔹 Validar que el club existe
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == entrenadorCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // Actualizar propiedades
                entrenador.IdClub = entrenadorCreateDto.IdClub;
                entrenador.Licencia = entrenadorCreateDto.Licencia;
                entrenador.PerteneceSeleccion = entrenadorCreateDto.PerteneceSeleccion;
                entrenador.CategoriaSeleccion = entrenadorCreateDto.CategoriaSeleccion;
                entrenador.BecadoEnard = entrenadorCreateDto.BecadoEnard;
                entrenador.BecadoSdn = entrenadorCreateDto.BecadoSdn;
                entrenador.MontoBeca = entrenadorCreateDto.MontoBeca;
                entrenador.PresentoAptoMedico = entrenadorCreateDto.PresentoAptoMedico;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntrenadorExists(id))
                {
                    return NotFound($"Entrenador con ID {id} no existe");
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

        // DELETE: api/Entrenador/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntrenador(int id)
        {
            try
            {
                var entrenador = await _context.Entrenadores.FindAsync(id);
                if (entrenador == null)
                {
                    return NotFound($"Entrenador con ID {id} no encontrado");
                }

                _context.Entrenadores.Remove(entrenador);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"No se puede eliminar el entrenador: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Entrenador/search?term=busqueda
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> SearchEntrenadores([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest("El término de búsqueda es requerido");
                }

                var entrenadores = await _context.Entrenadores
                    .Include(e => e.Persona)
                    .Include(e => e.Club)
                    .Where(e => e.Persona.Nombre.Contains(term) ||
                                e.Persona.Apellido.Contains(term) ||
                                e.Licencia.Contains(term))
                    .Select(e => new EntrenadorDto
                    {
                        IdPersona = e.IdPersona,
                        IdClub = e.IdClub,
                        Licencia = e.Licencia,
                        PerteneceSeleccion = e.PerteneceSeleccion,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard,
                        BecadoSdn = e.BecadoSdn,
                        MontoBeca = e.MontoBeca,
                        PresentoAptoMedico = e.PresentoAptoMedico,
                        NombrePersona = e.Persona.Nombre + " " + e.Persona.Apellido,
                        NombreClub = e.Club.Nombre,
                        SiglasClub = e.Club.Siglas
                    })
                    .ToListAsync();

                return Ok(entrenadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool EntrenadorExists(int id)
        {
            return _context.Entrenadores.Any(e => e.IdPersona == id);
        }


        // GET: api/Entrenador/seleccion
        [HttpGet("seleccion")]
        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresSeleccion()
        {
            try
            {
                var entrenadores = await _context.Entrenadores
                    .Include(e => e.Persona)
                    .Include(e => e.Club)
                    .Where(e => e.PerteneceSeleccion == true) // ← Solo entrenadores de selección
                    .Select(e => new EntrenadorDto
                    {
                        IdPersona = e.IdPersona,
                        IdClub = e.IdClub,
                        Licencia = e.Licencia,
                        PerteneceSeleccion = e.PerteneceSeleccion,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard,
                        BecadoSdn = e.BecadoSdn,
                        MontoBeca = e.MontoBeca,
                        PresentoAptoMedico = e.PresentoAptoMedico,
                        NombrePersona = e.Persona.Nombre + " " + e.Persona.Apellido,
                        NombreClub = e.Club.Nombre,
                        SiglasClub = e.Club.Siglas
                    })
                    .ToListAsync();

                return Ok(entrenadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}