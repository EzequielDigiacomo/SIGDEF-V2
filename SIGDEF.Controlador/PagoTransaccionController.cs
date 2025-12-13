using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.DTOs;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.DTOs.PagoTransaccion;
using SIGDEF.Entidades.DTOs.Persona;
using SIGDEF.Entidades.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagoTransaccionController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public PagoTransaccionController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/PagoTransaccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosTransaccion()
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Persona)
                    .Include(p => p.Club)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        IdPersona = p.IdPersona,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Persona.Nombre + " " + p.Persona.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PagoTransaccionDetailDto>> GetPagoTransaccion(int id)
        {
            try
            {
                var pago = await _context.PagosTransacciones
                    .Include(p => p.Persona)
                    .Include(p => p.Club)
                    .Where(p => p.IdPago == id)
                    .Select(p => new PagoTransaccionDetailDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        IdPersona = p.IdPersona,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        Persona = new PersonaDto
                        {
                            IdPersona = p.Persona.IdPersona,
                            Nombre = p.Persona.Nombre,
                            Apellido = p.Persona.Apellido,
                            Documento = p.Persona.Documento,
                            FechaNacimiento = p.Persona.FechaNacimiento,
                            Email = p.Persona.Email,
                            Telefono = p.Persona.Telefono,
                            Direccion = p.Persona.Direccion
                        },
                        Club = new ClubDto
                        {
                            IdClub = p.Club.IdClub,
                            Nombre = p.Club.Nombre,
                            Direccion = p.Club.Direccion,
                            Telefono = p.Club.Telefono,
                            Siglas = p.Club.Siglas
                        }
                    })
                    .FirstOrDefaultAsync();

                if (pago == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                return Ok(pago);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/persona/5
        [HttpGet("persona/{idPersona}")]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosPorPersona(int idPersona)
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Persona)
                    .Include(p => p.Club)
                    .Where(p => p.IdPersona == idPersona)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        IdPersona = p.IdPersona,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Persona.Nombre + " " + p.Persona.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/club/5
        [HttpGet("club/{idClub}")]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosPorClub(int idClub)
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Persona)
                    .Include(p => p.Club)
                    .Where(p => p.IdClub == idClub)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        IdPersona = p.IdPersona,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Persona.Nombre + " " + p.Persona.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/estado/pendiente
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosPorEstado(EstadoPagoTransaccion estado)
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Persona)
                    .Include(p => p.Club)
                    .Where(p => p.Estado == estado)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        IdPersona = p.IdPersona,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Persona.Nombre + " " + p.Persona.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/PagoTransaccion
        [HttpPost]
        public async Task<ActionResult<PagoTransaccionDto>> PostPagoTransaccion(PagoTransaccionCreateDto pagoTransaccionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que la persona existe
                var personaExists = await _context.Personas.AnyAsync(p => p.IdPersona == pagoTransaccionCreateDto.IdPersona);
                if (!personaExists)
                {
                    return BadRequest("La persona especificada no existe");
                }

                // 🔹 Validar que el club existe
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == pagoTransaccionCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                var pagoTransaccion = new PagoTransaccion
                {
                    Concepto = pagoTransaccionCreateDto.Concepto,
                    Monto = pagoTransaccionCreateDto.Monto,
                    Estado = pagoTransaccionCreateDto.Estado,
                    FechaCreacion = DateTime.UtcNow,
                    FechaAprobacion = pagoTransaccionCreateDto.Estado == EstadoPagoTransaccion.Aprobado ? DateTime.UtcNow : null,
                    IdPersona = pagoTransaccionCreateDto.IdPersona,
                    IdClub = pagoTransaccionCreateDto.IdClub,
                    IdMercadoPago = pagoTransaccionCreateDto.IdMercadoPago ?? string.Empty
                };

                _context.PagosTransacciones.Add(pagoTransaccion);
                await _context.SaveChangesAsync();

                // 🔹 Cargar datos relacionados para la respuesta
                await _context.Entry(pagoTransaccion)
                    .Reference(p => p.Persona)
                    .LoadAsync();
                await _context.Entry(pagoTransaccion)
                    .Reference(p => p.Club)
                    .LoadAsync();

                var pagoTransaccionDto = new PagoTransaccionDto
                {
                    IdPago = pagoTransaccion.IdPago,
                    Concepto = pagoTransaccion.Concepto,
                    Monto = pagoTransaccion.Monto,
                    Estado = pagoTransaccion.Estado,
                    FechaCreacion = pagoTransaccion.FechaCreacion,
                    FechaAprobacion = pagoTransaccion.FechaAprobacion,
                    IdPersona = pagoTransaccion.IdPersona,
                    IdClub = pagoTransaccion.IdClub,
                    IdMercadoPago = pagoTransaccion.IdMercadoPago,
                    NombrePersona = pagoTransaccion.Persona.Nombre + " " + pagoTransaccion.Persona.Apellido,
                    NombreClub = pagoTransaccion.Club.Nombre,
                    EstadoDescripcion = GetEstadoDescripcion(pagoTransaccion.Estado)
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetPagoTransaccion), new { id = pagoTransaccion.IdPago }, pagoTransaccionDto);
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

        // PUT: api/PagoTransaccion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPagoTransaccion(int id, PagoTransaccionCreateDto pagoTransaccionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var pagoTransaccion = await _context.PagosTransacciones.FindAsync(id);
                if (pagoTransaccion == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                // 🔹 Validar que la persona existe
                var personaExists = await _context.Personas.AnyAsync(p => p.IdPersona == pagoTransaccionCreateDto.IdPersona);
                if (!personaExists)
                {
                    return BadRequest("La persona especificada no existe");
                }

                // 🔹 Validar que el club existe
                var clubExists = await _context.Clubs.AnyAsync(c => c.IdClub == pagoTransaccionCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // Actualizar propiedades
                pagoTransaccion.Concepto = pagoTransaccionCreateDto.Concepto;
                pagoTransaccion.Monto = pagoTransaccionCreateDto.Monto;
                pagoTransaccion.Estado = pagoTransaccionCreateDto.Estado;
                pagoTransaccion.IdPersona = pagoTransaccionCreateDto.IdPersona;
                pagoTransaccion.IdClub = pagoTransaccionCreateDto.IdClub;
                pagoTransaccion.IdMercadoPago = pagoTransaccionCreateDto.IdMercadoPago ?? string.Empty;

                // 🔹 Si el estado cambia a Aprobado y no tiene fecha de aprobación, establecerla
                if (pagoTransaccionCreateDto.Estado == EstadoPagoTransaccion.Aprobado && !pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = DateTime.UtcNow;
                }
                // 🔹 Si el estado cambia de Aprobado a otro estado, limpiar la fecha de aprobación
                else if (pagoTransaccionCreateDto.Estado != EstadoPagoTransaccion.Aprobado && pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = null;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagoTransaccionExists(id))
                {
                    return NotFound($"PagoTransaccion con ID {id} no existe");
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

        // PATCH: api/PagoTransaccion/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstadoPago(int id, [FromBody] EstadoPagoTransaccion nuevoEstado)
        {
            try
            {
                var pagoTransaccion = await _context.PagosTransacciones.FindAsync(id);
                if (pagoTransaccion == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                pagoTransaccion.Estado = nuevoEstado;

                // 🔹 Actualizar fecha de aprobación si corresponde
                if (nuevoEstado == EstadoPagoTransaccion.Aprobado && !pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = DateTime.UtcNow;
                }
                else if (nuevoEstado != EstadoPagoTransaccion.Aprobado && pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = null;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE: api/PagoTransaccion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePagoTransaccion(int id)
        {
            try
            {
                var pagoTransaccion = await _context.PagosTransacciones.FindAsync(id);
                if (pagoTransaccion == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                _context.PagosTransacciones.Remove(pagoTransaccion);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar el pago: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/estadisticas
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticasPagos()
        {
            try
            {
                var estadisticas = new
                {
                    TotalPagos = await _context.PagosTransacciones.CountAsync(),
                    TotalMonto = await _context.PagosTransacciones.SumAsync(p => p.Monto),
                    PagosPendientes = await _context.PagosTransacciones.CountAsync(p => p.Estado == EstadoPagoTransaccion.Pendiente),
                    PagosAprobados = await _context.PagosTransacciones.CountAsync(p => p.Estado == EstadoPagoTransaccion.Aprobado),
                    PagosRechazados = await _context.PagosTransacciones.CountAsync(p => p.Estado == EstadoPagoTransaccion.Rechazado),
                    MontoAprobado = await _context.PagosTransacciones
                        .Where(p => p.Estado == EstadoPagoTransaccion.Aprobado)
                        .SumAsync(p => p.Monto)
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool PagoTransaccionExists(int id)
        {
            return _context.PagosTransacciones.Any(e => e.IdPago == id);
        }

        private string GetEstadoDescripcion(EstadoPagoTransaccion estado)
        {
            return estado switch
            {
                EstadoPagoTransaccion.Pendiente => "Pendiente",
                EstadoPagoTransaccion.Aprobado => "Aprobado",
                EstadoPagoTransaccion.Rechazado => "Rechazado",
                _ => "Desconocido"
            };
        }
    }
}