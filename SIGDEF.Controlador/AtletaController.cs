using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.AtletaTutor;
using SIGDEF.Entidades.DTOs.Persona;
using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.DTOs.Inscripcion;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SIGDEF.DTOs;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtletaController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public AtletaController(SIGDeFContext context)
        {
            _context = context;
        }

        // -------------------------------------------------
        // GET: api/Atleta
        // -------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtletaDto>>> GetAtletas()
        {
            try
            {
                var atletas = await _context.Atletas
                    .Include(a => a.Persona)
                    .Include(a => a.Club)
                    .Select(a => new AtletaDto
                    {
                        IdPersona = a.IdPersona,
                        IdClub = a.IdClub,
                        EstadoPago = a.EstadoPago,
                        PerteneceSeleccion = a.PerteneceSeleccion,
                        Categoria = a.Categoria,
                        BecadoEnard = a.BecadoEnard,
                        BecadoSdn = a.BecadoSdn,
                        MontoBeca = a.MontoBeca,
                        PresentoAptoMedico = a.PresentoAptoMedico,
                        FechaAptoMedico = a.FechaAptoMedico,
                        NombrePersona = a.Persona.Nombre + " " + a.Persona.Apellido,
                        NombreClub = a.Club.Nombre,
                        // NUEVA PROPIEDAD
                        FechaCreacion = a.FechaCreacion
                    })
                    .ToListAsync();

                return Ok(atletas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // -------------------------------------------------
        // GET: api/Atleta/5
        // -------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<AtletaDetailDto>> GetAtleta(int id)
        {
            try
            {
                var atleta = await _context.Atletas
                    .Include(a => a.Persona)
                    .Include(a => a.Club)
                    .Include(a => a.Inscripciones)
                        .ThenInclude(i => i.EventoPrueba)
                    .Include(a => a.Tutores)
                        .ThenInclude(at => at.Tutor)
                        .ThenInclude(t => t.Persona)
                    .Where(a => a.IdPersona == id)
                    .Select(a => new AtletaDetailDto
                    {
                        IdPersona = a.IdPersona,
                        IdClub = a.IdClub,
                        EstadoPago = a.EstadoPago,
                        PerteneceSeleccion = a.PerteneceSeleccion,
                        Categoria = a.Categoria,
                        BecadoEnard = a.BecadoEnard,
                        BecadoSdn = a.BecadoSdn,
                        MontoBeca = a.MontoBeca,
                        PresentoAptoMedico = a.PresentoAptoMedico,
                        FechaAptoMedico = a.FechaAptoMedico,
                        // NUEVA PROPIEDAD
                        FechaCreacion = a.FechaCreacion,

                        Persona = new PersonaDto
                        {
                            IdPersona = a.Persona.IdPersona,
                            Nombre = a.Persona.Nombre,
                            Apellido = a.Persona.Apellido,
                            Documento = a.Persona.Documento,
                            FechaNacimiento = a.Persona.FechaNacimiento,
                            Email = a.Persona.Email,
                            Telefono = a.Persona.Telefono,
                            Direccion = a.Persona.Direccion
                        },
                        Club = new ClubDto
                        {
                            IdClub = a.Club.IdClub,
                            Nombre = a.Club.Nombre,
                            Siglas = a.Club.Siglas
                        },
                        Inscripciones = a.Inscripciones.Select(i => new InscripcionDto
                        {
                            IdInscripcion = i.IdInscripcion,
                            IdEvento = i.IdEventoPrueba,
                            FechaInscripcion = i.FechaInscripcion,
                        }).ToList(),
                        Tutores = a.Tutores.Select(at => new AtletaTutorDto
                        {
                            IdAtleta = at.IdAtleta,
                            IdTutor = at.IdTutor,
                            Parentesco = at.Parentesco,
                            NombreTutor = at.Tutor.Persona.Nombre + " " + at.Tutor.Persona.Apellido
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (atleta == null)
                {
                    return NotFound($"Atleta con ID {id} no encontrado");
                }

                return Ok(atleta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // -------------------------------------------------
        // POST: api/Atleta
        // -------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<AtletaDto>> PostAtleta(AtletaCreateDto atletaCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar existencia de Persona
                var personaExists = await _context.Personas.AnyAsync(p => p.IdPersona == atletaCreateDto.IdPersona);
                if (!personaExists)
                {
                    return BadRequest("La persona especificada no existe");
                }

                // Validar existencia de Club
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == atletaCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // Evitar duplicados
                var atletaExists = await _context.Atletas.AnyAsync(a => a.IdPersona == atletaCreateDto.IdPersona);
                if (atletaExists)
                {
                    return BadRequest("Esta persona ya está registrada como atleta");
                }

                // Convertir fecha de apto médico a UTC (si corresponde)
                DateTime? fechaAptoMedicoUtc = null;
                if (atletaCreateDto.FechaAptoMedico.HasValue)
                {
                    fechaAptoMedicoUtc = DateTime.SpecifyKind(atletaCreateDto.FechaAptoMedico.Value, DateTimeKind.Utc);
                }

                var atleta = new Atleta
                {
                    IdPersona = atletaCreateDto.IdPersona,
                    IdClub = atletaCreateDto.IdClub,
                    EstadoPago = atletaCreateDto.EstadoPago,
                    PerteneceSeleccion = atletaCreateDto.PerteneceSeleccion,
                    Categoria = atletaCreateDto.Categoria,
                    BecadoEnard = atletaCreateDto.BecadoEnard,
                    BecadoSdn = atletaCreateDto.BecadoSdn,
                    MontoBeca = atletaCreateDto.MontoBeca,
                    PresentoAptoMedico = atletaCreateDto.PresentoAptoMedico,
                    FechaAptoMedico = fechaAptoMedicoUtc,
                    // **FechaCreacion** se asigna automáticamente por la entidad (DateTime.UtcNow)
                };

                _context.Atletas.Add(atleta);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(atleta).Reference(a => a.Persona).LoadAsync();
                await _context.Entry(atleta).Reference(a => a.Club).LoadAsync();

                var atletaDto = new AtletaDto
                {
                    IdPersona = atleta.IdPersona,
                    IdClub = atleta.IdClub,
                    EstadoPago = atleta.EstadoPago,
                    PerteneceSeleccion = atleta.PerteneceSeleccion,
                    Categoria = atleta.Categoria,
                    BecadoEnard = atleta.BecadoEnard,
                    BecadoSdn = atleta.BecadoSdn,
                    MontoBeca = atleta.MontoBeca,
                    PresentoAptoMedico = atleta.PresentoAptoMedico,
                    FechaAptoMedico = atleta.FechaAptoMedico,
                    NombrePersona = atleta.Persona.Nombre + " " + atleta.Persona.Apellido,
                    NombreClub = atleta.Club.Nombre,
                    // NUEVA PROPIEDAD
                    FechaCreacion = atleta.FechaCreacion
                };

                // 201 Created
                return CreatedAtAction(nameof(GetAtleta), new { id = atleta.IdPersona }, atletaDto);
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

        // -------------------------------------------------
        // PUT: api/Atleta/5
        // -------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAtleta(int id, AtletaCreateDto atletaCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != atletaCreateDto.IdPersona)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del atleta");
                }

                var atleta = await _context.Atletas.FindAsync(id);
                if (atleta == null)
                {
                    return NotFound($"Atleta con ID {id} no encontrado");
                }

                // Validar club
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == atletaCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // Convertir fecha apto médico a UTC
                DateTime? fechaAptoMedicoUtc = null;
                if (atletaCreateDto.FechaAptoMedico.HasValue)
                {
                    fechaAptoMedicoUtc = DateTime.SpecifyKind(atletaCreateDto.FechaAptoMedico.Value, DateTimeKind.Utc);
                }

                // Actualizar campos (FechaCreacion **no** se modifica)
                atleta.IdClub = atletaCreateDto.IdClub;
                atleta.EstadoPago = atletaCreateDto.EstadoPago;
                atleta.PerteneceSeleccion = atletaCreateDto.PerteneceSeleccion;
                atleta.Categoria = atletaCreateDto.Categoria;
                atleta.BecadoEnard = atletaCreateDto.BecadoEnard;
                atleta.BecadoSdn = atletaCreateDto.BecadoSdn;
                atleta.MontoBeca = atletaCreateDto.MontoBeca;
                atleta.PresentoAptoMedico = atletaCreateDto.PresentoAptoMedico;
                atleta.FechaAptoMedico = fechaAptoMedicoUtc;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AtletaExistsAsync(id))
                {
                    return NotFound($"Atleta con ID {id} no existe");
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

        // -------------------------------------------------
        // DELETE: api/Atleta/5
        // -------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAtleta(int id)
        {
            try
            {
                var atleta = await _context.Atletas
                    .Include(a => a.Persona)
                    .Include(a => a.Tutores)
                    .Include(a => a.Inscripciones)
                    .FirstOrDefaultAsync(a => a.IdPersona == id);

                if (atleta == null)
                {
                    return NotFound($"Atleta con ID {id} no encontrado");
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1️⃣ Eliminar relaciones Atleta‑Tutor (solo la tabla intermedia)
                    if (atleta.Tutores.Any())
                    {
                        _context.AtletasTutores.RemoveRange(atleta.Tutores);
                    }

                    // 2️⃣ Eliminar inscripciones del atleta
                    if (atleta.Inscripciones.Any())
                    {
                        _context.Inscripciones.RemoveRange(atleta.Inscripciones);
                    }

                    // 3️⃣ Eliminar el registro de atleta
                    _context.Atletas.Remove(atleta);

                    // 4️⃣ Verificar si la persona tiene otros roles antes de borrarla
                    var persona = atleta.Persona;
                    var tieneOtrosRoles = await _context.Usuarios.AnyAsync(u => u.IdPersona == id) ||
                                         await _context.Entrenadores.AnyAsync(e => e.IdPersona == id) ||
                                         await _context.DelegadosClub.AnyAsync(d => d.IdPersona == id) ||
                                         await _context.Tutores.AnyAsync(t => t.IdPersona == id);

                    // 5️⃣ Eliminar la persona SOLO si no tiene otros roles
                    if (!tieneOtrosRoles)
                    {
                        // Eliminar pagos asociados a la persona (si existen)
                        var pagosPersona = await _context.PagosTransacciones
                            .Where(p => p.IdPersona == id)
                            .ToListAsync();

                        if (pagosPersona.Any())
                        {
                            _context.PagosTransacciones.RemoveRange(pagosPersona);
                        }

                        _context.Personas.Remove(persona);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var mensaje = tieneOtrosRoles
                        ? "Atleta eliminado, pero la persona se mantiene porque tiene otros roles"
                        : "Atleta y persona eliminados completamente";

                    return Ok(new { message = mensaje });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar el atleta: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // -------------------------------------------------
        // Métodos auxiliares
        // -------------------------------------------------
        private async Task<bool> AtletaExistsAsync(int id)
        {
            return await _context.Atletas.AnyAsync(e => e.IdPersona == id);
        }
    }
}