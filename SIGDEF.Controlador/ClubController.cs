using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.DTOs;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.Entrenador;
using SIGDEF.Entidades.DTOs.DelegadoClub;
using SIGDEF.Entidades.DTOs.PagoTransaccion;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public ClubController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Club
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubes()
        {
            try
            {
                var clubes = await _context.Clubs
                    .Select(c => new ClubDto
                    {
                        IdClub = c.IdClub,
                        Nombre = c.Nombre,
                        Direccion = c.Direccion,
                        Telefono = c.Telefono,
                        Siglas = c.Siglas,
                        CantidadAtletas = c.Atletas.Count,
                        CantidadEntrenadores = c.Entrenadores.Count,
                        CantidadRepresentantes = c.Representantes.Count
                    })
                    .ToListAsync();

                return Ok(clubes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Club/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClubDetailDto>> GetClub(int id)
        {
            try
            {
                var club = await _context.Clubs
                    .Include(c => c.Atletas)
                        .ThenInclude(a => a.Persona)
                    .Include(c => c.Entrenadores)
                        .ThenInclude(e => e.Persona)
                    .Include(c => c.Representantes)
                        .ThenInclude(r => r.Persona)
                    .Include(c => c.Pagos)
                    .Where(c => c.IdClub == id)
                    .Select(c => new ClubDetailDto
                    {
                        IdClub = c.IdClub,
                        Nombre = c.Nombre,
                        Direccion = c.Direccion,
                        Telefono = c.Telefono,
                        Siglas = c.Siglas,
                        Atletas = c.Atletas.Select(a => new AtletaDto
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
                            NombrePersona = a.Persona.Nombre + " " + a.Persona.Apellido
                        }).ToList(),
                        Entrenadores = c.Entrenadores.Select(e => new EntrenadorDto
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
                            NombrePersona = e.Persona.Nombre + " " + e.Persona.Apellido
                        }).ToList(),
                        Representantes = c.Representantes.Select(r => new DelegadoClubDto
                        {
                            IdPersona = r.IdPersona,
                            IdRol = r.IdRol,
                            IdFederacion = r.IdFederacion,
                            NombrePersona = r.Persona.Nombre + " " + r.Persona.Apellido
                        }).ToList(),
                        Pagos = c.Pagos.Select(p => new PagoTransaccionDto
                        {
                            IdPago = p.IdPago,
                            Concepto = p.Concepto,
                            Monto = p.Monto,
                            Estado = p.Estado,
                            FechaCreacion = p.FechaCreacion,
                            FechaAprobacion = p.FechaAprobacion,
                            IdPersona = p.IdPersona,
                            IdClub = p.IdClub,
                            IdMercadoPago = p.IdMercadoPago
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (club == null)
                {
                    return NotFound($"Club con ID {id} no encontrado");
                }

                return Ok(club);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Club
        [HttpPost]
        public async Task<ActionResult<ClubDto>> PostClub(ClubCreateDto clubCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que las siglas no existan
                var siglasExists = await _context.Clubs.AnyAsync(c => c.Siglas == clubCreateDto.Siglas);
                if (siglasExists)
                {
                    return BadRequest("Ya existe un club con estas siglas");
                }

                // 🔹 Validar que el nombre no exista
                var nombreExists = await _context.Clubs.AnyAsync(c => c.Nombre == clubCreateDto.Nombre);
                if (nombreExists)
                {
                    return BadRequest("Ya existe un club con este nombre");
                }

                var club = new Club
                {
                    Nombre = clubCreateDto.Nombre,
                    Direccion = clubCreateDto.Direccion ?? string.Empty,
                    Telefono = clubCreateDto.Telefono ?? string.Empty,
                    Siglas = clubCreateDto.Siglas
                };

                _context.Clubs.Add(club);
                await _context.SaveChangesAsync();

                var clubDto = new ClubDto
                {
                    IdClub = club.IdClub,
                    Nombre = club.Nombre,
                    Direccion = club.Direccion,
                    Telefono = club.Telefono,
                    Siglas = club.Siglas,
                    CantidadAtletas = 0,
                    CantidadEntrenadores = 0,
                    CantidadRepresentantes = 0
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetClub), new { id = club.IdClub }, clubDto);
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

        // PUT: api/Club/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClub(int id, ClubCreateDto clubCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var club = await _context.Clubs.FindAsync(id);
                if (club == null)
                {
                    return NotFound($"Club con ID {id} no encontrado");
                }

                // 🔹 Validar que las siglas no existan (excluyendo el actual)
                var siglasExists = await _context.Clubs
                    .AnyAsync(c => c.Siglas == clubCreateDto.Siglas && c.IdClub != id);
                if (siglasExists)
                {
                    return BadRequest("Ya existe otro club con estas siglas");
                }

                // 🔹 Validar que el nombre no exista (excluyendo el actual)
                var nombreExists = await _context.Clubs
                    .AnyAsync(c => c.Nombre == clubCreateDto.Nombre && c.IdClub != id);
                if (nombreExists)
                {
                    return BadRequest("Ya existe otro club con este nombre");
                }

                // Actualizar propiedades
                club.Nombre = clubCreateDto.Nombre;
                club.Direccion = clubCreateDto.Direccion ?? string.Empty;
                club.Telefono = clubCreateDto.Telefono ?? string.Empty;
                club.Siglas = clubCreateDto.Siglas;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExists(id))
                {
                    return NotFound($"Club con ID {id} no existe");
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

        // DELETE: api/Club/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            try
            {
                var club = await _context.Clubs
                    .Include(c => c.Atletas)
                    .Include(c => c.Entrenadores)
                    .Include(c => c.Representantes)
                    .Include(c => c.Pagos)
                    .FirstOrDefaultAsync(c => c.IdClub == id);

                if (club == null)
                {
                    return NotFound($"Club con ID {id} no encontrado");
                }

                // 🔹 Validar que no tenga datos relacionados
                if (club.Atletas.Any())
                {
                    return BadRequest("No se puede eliminar el club porque tiene atletas asociados");
                }

                if (club.Entrenadores.Any())
                {
                    return BadRequest("No se puede eliminar el club porque tiene entrenadores asociados");
                }

                if (club.Representantes.Any())
                {
                    return BadRequest("No se puede eliminar el club porque tiene representantes asociados");
                }

                if (club.Pagos.Any())
                {
                    return BadRequest("No se puede eliminar el club porque tiene pagos asociados");
                }

                _context.Clubs.Remove(club);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar el club: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Club/search?term=busqueda
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClubDto>>> SearchClubes([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest("El término de búsqueda es requerido");
                }

                var clubes = await _context.Clubs
                    .Where(c => c.Nombre.Contains(term) || c.Siglas.Contains(term))
                    .Select(c => new ClubDto
                    {
                        IdClub = c.IdClub,
                        Nombre = c.Nombre,
                        Direccion = c.Direccion,
                        Telefono = c.Telefono,
                        Siglas = c.Siglas,
                        CantidadAtletas = c.Atletas.Count,
                        CantidadEntrenadores = c.Entrenadores.Count,
                        CantidadRepresentantes = c.Representantes.Count
                    })
                    .ToListAsync();

                return Ok(clubes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool ClubExists(int id)
        {
            return _context.Clubs.Any(e => e.IdClub == id);
        }
    }
}