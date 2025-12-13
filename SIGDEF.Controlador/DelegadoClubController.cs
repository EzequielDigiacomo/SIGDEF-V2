using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades.DTOs.Persona;
using SIGDEF.Entidades.DTOs.DelegadoClub;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Rol;
using SIGDEF.Entidades.DTOs.Federacion;
using SIGDEF.Entidades.DTOs.Club;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DelegadoClubController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public DelegadoClubController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/DelegadoClub
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosClub()
        {
            try
            {
                var delegados = await _context.DelegadosClub
                    .Include(d => d.Persona)
                    .Include(d => d.Rol)
                    .Include(d => d.Federacion)
                    .Include(d => d.Club)
                    .Select(d => new DelegadoClubDto
                    {
                        IdPersona = d.IdPersona,
                        IdRol = d.IdRol,
                        IdFederacion = d.IdFederacion,
                        IdClub = d.ClubIdClub,
                        NombrePersona = d.Persona.Nombre + " " + d.Persona.Apellido,
                        TipoRol = d.Rol.Tipo,
                        NombreFederacion = d.Federacion.Nombre,
                        NombreClub = d.Club.Nombre,
                        Documento = d.Persona.Documento,
                        Email = d.Persona.Email,
                        Telefono = d.Persona.Telefono
                    })
                    .ToListAsync();

                return Ok(delegados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/DelegadoClub/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DelegadoClubDetailDto>> GetDelegadoClub(int id)
        {
            try
            {
                var delegado = await _context.DelegadosClub
                    .Include(d => d.Persona)
                    .Include(d => d.Rol)
                    .Include(d => d.Federacion)
                    .Include(d => d.Club)
                    .Where(d => d.IdPersona == id)
                    .Select(d => new DelegadoClubDetailDto
                    {
                        IdPersona = d.IdPersona,
                        IdRol = d.IdRol,
                        IdFederacion = d.IdFederacion,
                        IdClub = d.ClubIdClub,
                        Persona = new PersonaDto
                        {
                            IdPersona = d.Persona.IdPersona,
                            Nombre = d.Persona.Nombre,
                            Apellido = d.Persona.Apellido,
                            Documento = d.Persona.Documento,
                            FechaNacimiento = d.Persona.FechaNacimiento,
                            Email = d.Persona.Email,
                            Telefono = d.Persona.Telefono,
                            Direccion = d.Persona.Direccion
                        },
                        Rol = new RolDto
                        {
                            IdRol = d.Rol.IdRol,
                            Tipo = d.Rol.Tipo
                        },
                        Federacion = new FederacionDto
                        {
                            IdFederacion = d.Federacion.IdFederacion,
                            Nombre = d.Federacion.Nombre,
                            Cuit = d.Federacion.Cuit,
                            Email = d.Federacion.Email,
                            Telefono = d.Federacion.Telefono
                        },
                        Club = new ClubDto
                        {
                            IdClub = d.Club.IdClub,
                            Nombre = d.Club.Nombre,
                            Direccion = d.Club.Direccion,
                            Telefono = d.Club.Telefono,
                            Siglas = d.Club.Siglas
                        }
                    })
                    .FirstOrDefaultAsync();

                if (delegado == null)
                {
                    return NotFound($"DelegadoClub con ID {id} no encontrado");
                }

                return Ok(delegado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/DelegadoClub/federacion/5
        [HttpGet("federacion/{idFederacion}")]
        public async Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosPorFederacion(int idFederacion)
        {
            try
            {
                var delegados = await _context.DelegadosClub
                    .Include(d => d.Persona)
                    .Include(d => d.Rol)
                    .Include(d => d.Federacion)
                    .Include(d => d.Club)
                    .Where(d => d.IdFederacion == idFederacion)
                    .Select(d => new DelegadoClubDto
                    {
                        IdPersona = d.IdPersona,
                        IdRol = d.IdRol,
                        IdFederacion = d.IdFederacion,
                        IdClub = d.ClubIdClub,
                        NombrePersona = d.Persona.Nombre + " " + d.Persona.Apellido,
                        TipoRol = d.Rol.Tipo,
                        NombreFederacion = d.Federacion.Nombre,
                        NombreClub = d.Club.Nombre,
                        Documento = d.Persona.Documento,
                        Email = d.Persona.Email,
                        Telefono = d.Persona.Telefono
                    })
                    .ToListAsync();

                return Ok(delegados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/DelegadoClub
        [HttpPost]
        public async Task<ActionResult<DelegadoClubDto>> PostDelegadoClub(DelegadoClubCreateDto delegadoClubCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que la persona existe
                var personaExists = await _context.Personas.AnyAsync(p => p.IdPersona == delegadoClubCreateDto.IdPersona);
                if (!personaExists)
                {
                    return BadRequest("La persona especificada no existe");
                }

                // 🔹 Validar que el rol existe
                var rolExists = await _context.Roles.AnyAsync(r => r.IdRol == delegadoClubCreateDto.IdRol);
                if (!rolExists)
                {
                    return BadRequest("El rol especificado no existe");
                }

                // 🔹 Validar que la federación existe
                var federacionExists = await _context.Federaciones.AnyAsync(f => f.IdFederacion == delegadoClubCreateDto.IdFederacion);
                if (!federacionExists)
                {
                    return BadRequest("La federación especificada no existe");
                }

                // 🔹 Validar que el club existe
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == delegadoClubCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // 🔹 Validar que no sea delegado ya
                var delegadoExists = await _context.DelegadosClub.AnyAsync(d => d.IdPersona == delegadoClubCreateDto.IdPersona);
                if (delegadoExists)
                {
                    return BadRequest("Esta persona ya está registrada como delegado de club");
                }

                var delegadoClub = new DelegadoClub
                {
                    IdPersona = delegadoClubCreateDto.IdPersona,
                    IdRol = delegadoClubCreateDto.IdRol,
                    IdFederacion = delegadoClubCreateDto.IdFederacion,
                    ClubIdClub = delegadoClubCreateDto.IdClub
                };

                _context.DelegadosClub.Add(delegadoClub);
                await _context.SaveChangesAsync();

                // 🔹 Cargar datos relacionados para la respuesta
                await _context.Entry(delegadoClub)
                    .Reference(d => d.Persona)
                    .LoadAsync();
                await _context.Entry(delegadoClub)
                    .Reference(d => d.Rol)
                    .LoadAsync();
                await _context.Entry(delegadoClub)
                    .Reference(d => d.Federacion)
                    .LoadAsync();
                await _context.Entry(delegadoClub)
                    .Reference(d => d.Club)
                    .LoadAsync();

                var delegadoClubDto = new DelegadoClubDto
                {
                    IdPersona = delegadoClub.IdPersona,
                    IdRol = delegadoClub.IdRol,
                    IdFederacion = delegadoClub.IdFederacion,
                    IdClub = delegadoClub.ClubIdClub,
                    NombrePersona = delegadoClub.Persona.Nombre + " " + delegadoClub.Persona.Apellido,
                    TipoRol = delegadoClub.Rol.Tipo,
                    NombreFederacion = delegadoClub.Federacion.Nombre,
                    NombreClub = delegadoClub.Club.Nombre
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetDelegadoClub), new { id = delegadoClub.IdPersona }, delegadoClubDto);
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

        // PUT: api/DelegadoClub/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDelegadoClub(int id, DelegadoClubCreateDto delegadoClubCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != delegadoClubCreateDto.IdPersona)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del delegado");
                }

                var delegadoClub = await _context.DelegadosClub.FindAsync(id);
                if (delegadoClub == null)
                {
                    return NotFound($"DelegadoClub con ID {id} no encontrado");
                }

                // 🔹 Validar que el rol existe
                var rolExists = await _context.Roles.AnyAsync(r => r.IdRol == delegadoClubCreateDto.IdRol);
                if (!rolExists)
                {
                    return BadRequest("El rol especificado no existe");
                }

                // 🔹 Validar que la federación existe
                var federacionExists = await _context.Federaciones.AnyAsync(f => f.IdFederacion == delegadoClubCreateDto.IdFederacion);
                if (!federacionExists)
                {
                    return BadRequest("La federación especificada no existe");
                }

                // 🔹 Validar que el club existe
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == delegadoClubCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // Actualizar propiedades (el IdPersona no se puede modificar ya que es la clave)
                delegadoClub.IdRol = delegadoClubCreateDto.IdRol;
                delegadoClub.IdFederacion = delegadoClubCreateDto.IdFederacion;
                delegadoClub.ClubIdClub = delegadoClubCreateDto.IdClub;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DelegadoClubExists(id))
                {
                    return NotFound($"DelegadoClub con ID {id} no existe");
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

        // DELETE: api/DelegadoClub/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelegadoClub(int id)
        {
            try
            {
                var delegadoClub = await _context.DelegadosClub.FindAsync(id);
                if (delegadoClub == null)
                {
                    return NotFound($"DelegadoClub con ID {id} no encontrado");
                }

                _context.DelegadosClub.Remove(delegadoClub);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"No se puede eliminar el delegado: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool DelegadoClubExists(int id)
        {
            return _context.DelegadosClub.Any(e => e.IdPersona == id);
        }
    }
}