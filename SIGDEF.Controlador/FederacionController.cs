using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SIGDEF.Entidades.DTOs.Federacion;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FederacionController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public FederacionController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Federacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FederacionDto>>> GetFederaciones()
        {
            try
            {
                var federaciones = await _context.Federaciones
                    .Select(f => new FederacionDto
                    {
                        IdFederacion = f.IdFederacion,
                        Nombre = f.Nombre,
                        Cuit = f.Cuit,
                        Email = f.Email,
                        Telefono = f.Telefono,
                        Direccion = f.Direccion,
                        BancoNombre = f.BancoNombre,
                        TipoCuenta = f.TipoCuenta,
                        NumeroCuenta = f.NumeroCuenta,
                        TitularCuenta = f.TitularCuenta,
                        EmailCobro = f.EmailCobro
                    })
                    .ToListAsync();

                return Ok(federaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Federacion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FederacionDto>> GetFederacion(int id)
        {
            try
            {
                var federacion = await _context.Federaciones
                    .Where(f => f.IdFederacion == id)
                    .Select(f => new FederacionDto
                    {
                        IdFederacion = f.IdFederacion,
                        Nombre = f.Nombre,
                        Cuit = f.Cuit,
                        Email = f.Email,
                        Telefono = f.Telefono,
                        Direccion = f.Direccion,
                        BancoNombre = f.BancoNombre,
                        TipoCuenta = f.TipoCuenta,
                        NumeroCuenta = f.NumeroCuenta,
                        TitularCuenta = f.TitularCuenta,
                        EmailCobro = f.EmailCobro
                    })
                    .FirstOrDefaultAsync();

                if (federacion == null)
                {
                    return NotFound($"Federación con ID {id} no encontrada");
                }

                return Ok(federacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Federacion
        [HttpPost]
        public async Task<ActionResult<FederacionDto>> PostFederacion(FederacionCreateDto federacionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 CORREGIDO: Validar campos requeridos explícitamente
                if (string.IsNullOrWhiteSpace(federacionCreateDto.Nombre))
                    return BadRequest("El nombre es requerido");

                if (string.IsNullOrWhiteSpace(federacionCreateDto.Cuit))
                    return BadRequest("El CUIT es requerido");

                var federacion = new Federacion
                {
                    Nombre = federacionCreateDto.Nombre,
                    Cuit = federacionCreateDto.Cuit,
                    Email = federacionCreateDto.Email ?? string.Empty,        // 🔹 CORREGIDO: Manejar null
                    Telefono = federacionCreateDto.Telefono ?? string.Empty,  // 🔹 CORREGIDO: Manejar null
                    Direccion = federacionCreateDto.Direccion ?? string.Empty, // 🔹 CORREGIDO: Manejar null
                    BancoNombre = federacionCreateDto.BancoNombre ?? string.Empty,
                    TipoCuenta = federacionCreateDto.TipoCuenta ?? string.Empty,
                    NumeroCuenta = federacionCreateDto.NumeroCuenta ?? string.Empty,
                    TitularCuenta = federacionCreateDto.TitularCuenta ?? string.Empty,
                    EmailCobro = federacionCreateDto.EmailCobro ?? string.Empty
                };

                _context.Federaciones.Add(federacion);
                await _context.SaveChangesAsync();

                var federacionDto = new FederacionDto
                {
                    IdFederacion = federacion.IdFederacion,
                    Nombre = federacion.Nombre,
                    Cuit = federacion.Cuit,
                    Email = federacion.Email,
                    Telefono = federacion.Telefono,
                    Direccion = federacion.Direccion,
                    BancoNombre = federacion.BancoNombre,
                    TipoCuenta = federacion.TipoCuenta,
                    NumeroCuenta = federacion.NumeroCuenta,
                    TitularCuenta = federacion.TitularCuenta,
                    EmailCobro = federacion.EmailCobro
                };

                // 🔹 CORREGIDO: Esto ya retorna 201 Created - está correcto
                return CreatedAtAction(nameof(GetFederacion), new { id = federacion.IdFederacion }, federacionDto);
            }
            catch (DbUpdateException dbEx)
            {
                // 🔹 CORREGIDO: Manejo específico de errores de base de datos
                return StatusCode(500, $"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Federacion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFederacion(int id, FederacionCreateDto federacionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 CORREGIDO: Validar campos requeridos en PUT también
                if (string.IsNullOrWhiteSpace(federacionCreateDto.Nombre))
                    return BadRequest("El nombre es requerido");

                if (string.IsNullOrWhiteSpace(federacionCreateDto.Cuit))
                    return BadRequest("El CUIT es requerido");

                var federacion = await _context.Federaciones.FindAsync(id);
                if (federacion == null)
                {
                    return NotFound($"Federación con ID {id} no encontrada");
                }

                // Actualizar propiedades
                federacion.Nombre = federacionCreateDto.Nombre;
                federacion.Cuit = federacionCreateDto.Cuit;
                federacion.Email = federacionCreateDto.Email ?? string.Empty;        // 🔹 CORREGIDO: Manejar null
                federacion.Telefono = federacionCreateDto.Telefono ?? string.Empty;  // 🔹 CORREGIDO: Manejar null
                federacion.Direccion = federacionCreateDto.Direccion ?? string.Empty;
                federacion.BancoNombre = federacionCreateDto.BancoNombre ?? string.Empty;
                federacion.TipoCuenta = federacionCreateDto.TipoCuenta ?? string.Empty;
                federacion.NumeroCuenta = federacionCreateDto.NumeroCuenta ?? string.Empty;
                federacion.TitularCuenta = federacionCreateDto.TitularCuenta ?? string.Empty;
                federacion.EmailCobro = federacionCreateDto.EmailCobro ?? string.Empty;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 🔹 CORREGIDO: Manejo específico para concurrencia
                if (!FederacionExists(id))
                {
                    return NotFound($"Federación con ID {id} no existe");
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

        // DELETE: api/Federacion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFederacion(int id)
        {
            try
            {
                var federacion = await _context.Federaciones.FindAsync(id);
                if (federacion == null)
                {
                    return NotFound($"Federación con ID {id} no encontrada");
                }

                _context.Federaciones.Remove(federacion);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                // 🔹 CORREGIDO: Manejo específico para errores de eliminación (FK constraints)
                return StatusCode(500, $"No se puede eliminar la federación porque tiene datos relacionados: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // 🔹 CORREGIDO: Agregar método auxiliar que faltaba
        private bool FederacionExists(int id)
        {
            return _context.Federaciones.Any(e => e.IdFederacion == id);
        }
    }
}