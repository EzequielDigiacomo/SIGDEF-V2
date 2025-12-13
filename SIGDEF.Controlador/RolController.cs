using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Extensions; // 👈 Agregar este using
using SIGDEF.DTOs;
using SIGDEF.Entidades;
using SIGDEF.Entidades.Enums; // 👈 Agregar este using
using SIGDEF.Entidades.DTOs.DelegadoClub;
using SIGDEF.Entidades.DTOs.Rol;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public RolController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Rol
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
        {
            try
            {
                var roles = await _context.Roles
                    .Select(r => new RolDto
                    {
                        IdRol = r.IdRol,
                        Tipo = r.Tipo,
                        TipoEnum = r.TipoEnum, // 👈 Agregar propiedad enum
                        CantidadRepresentantes = r.DelegadosClub.Count
                    })
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Rol/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolDetailDto>> GetRol(int id)
        {
            try
            {
                var rol = await _context.Roles
                    .Include(r => r.DelegadosClub)
                        .ThenInclude(d => d.Persona)
                    .Include(r => r.DelegadosClub)
                        .ThenInclude(d => d.Federacion)
                    .Where(r => r.IdRol == id)
                    .Select(r => new RolDetailDto
                    {
                        IdRol = r.IdRol,
                        Tipo = r.Tipo,
                        TipoEnum = r.TipoEnum, // 👈 Agregar propiedad enum
                        Representantes = r.DelegadosClub.Select(d => new DelegadoClubDto
                        {
                            IdPersona = d.IdPersona,
                            IdRol = d.IdRol,
                            IdFederacion = d.IdFederacion,
                            NombrePersona = d.Persona.Nombre + " " + d.Persona.Apellido,
                            NombreFederacion = d.Federacion.Nombre
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (rol == null)
                {
                    return NotFound($"Rol con ID {id} no encontrado");
                }

                return Ok(rol);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Rol
        [HttpPost]
        public async Task<ActionResult<RolDto>> PostRol(RolCreateDto rolCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que el tipo sea un enum válido
                if (!Enum.TryParse<RolTipo>(rolCreateDto.Tipo, true, out var rolTipo))
                {
                    return BadRequest($"Tipo de rol inválido: {rolCreateDto.Tipo}. Tipos válidos: {string.Join(", ", Enum.GetNames(typeof(RolTipo)))}");
                }

                // 🔹 Validar que no exista un rol con el mismo tipo (usando extensiones)
                var rolExistente = await _context.Roles
                    .ExistsByTipoAsync(rolTipo); // 👈 Usando extensión

                if (rolExistente)
                {
                    return BadRequest($"Ya existe un rol con el tipo '{rolCreateDto.Tipo}'");
                }

                var rol = new Rol
                {
                    Tipo = rolCreateDto.Tipo // Se guarda como string en la DB
                };

                _context.Roles.Add(rol);
                await _context.SaveChangesAsync();

                var rolDto = new RolDto
                {
                    IdRol = rol.IdRol,
                    Tipo = rol.Tipo,
                    TipoEnum = rol.TipoEnum, // 👈 Agregar propiedad enum
                    CantidadRepresentantes = 0
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetRol), new { id = rol.IdRol }, rolDto);
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

        // PUT: api/Rol/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, RolCreateDto rolCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que el tipo sea un enum válido
                if (!Enum.TryParse<RolTipo>(rolCreateDto.Tipo, true, out var rolTipo))
                {
                    return BadRequest($"Tipo de rol inválido: {rolCreateDto.Tipo}. Tipos válidos: {string.Join(", ", Enum.GetNames(typeof(RolTipo)))}");
                }

                var rol = await _context.Roles.FindAsync(id);
                if (rol == null)
                {
                    return NotFound($"Rol con ID {id} no encontrado");
                }

                // 🔹 Validar que no exista otro rol con el mismo tipo (usando extensiones)
                var otroRolConMismoTipo = await _context.Roles
                    .GetByTipoAsync(rolTipo); // 👈 Usando extensión

                if (otroRolConMismoTipo != null && otroRolConMismoTipo.IdRol != id)
                {
                    return BadRequest($"Ya existe otro rol con el tipo '{rolCreateDto.Tipo}' (ID: {otroRolConMismoTipo.IdRol})");
                }

                // Actualizar propiedades
                rol.Tipo = rolCreateDto.Tipo;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
                {
                    return NotFound($"Rol con ID {id} no existe");
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

        // DELETE: api/Rol/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            try
            {
                var rol = await _context.Roles
                    .Include(r => r.DelegadosClub)
                    .FirstOrDefaultAsync(r => r.IdRol == id);

                if (rol == null)
                {
                    return NotFound($"Rol con ID {id} no encontrado");
                }

                // 🔹 Validar que no tenga representantes asociados
                if (rol.DelegadosClub.Any())
                {
                    return BadRequest("No se puede eliminar el rol porque tiene representantes asociados");
                }

                _context.Roles.Remove(rol);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar el rol: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Rol/search?term=busqueda
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RolDto>>> SearchRoles([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest("El término de búsqueda es requerido");
                }

                var roles = await _context.Roles
                    .Where(r => r.Tipo.Contains(term))
                    .Select(r => new RolDto
                    {
                        IdRol = r.IdRol,
                        Tipo = r.Tipo,
                        TipoEnum = r.TipoEnum, // 👈 Agregar propiedad enum
                        CantidadRepresentantes = r.DelegadosClub.Count
                    })
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Rol/tipo/Administrador
        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<RolDto>> GetRolPorTipo(string tipo)
        {
            try
            {
                // 🔹 Primero intentar parsear como enum
                if (Enum.TryParse<RolTipo>(tipo, true, out var rolTipo))
                {
                    // Usar extensión para buscar por enum
                    var rol = await _context.Roles.GetByTipoAsync(rolTipo);

                    if (rol == null)
                    {
                        return NotFound($"Rol con tipo '{tipo}' no encontrado");
                    }

                    var rolDto = new RolDto
                    {
                        IdRol = rol.IdRol,
                        Tipo = rol.Tipo,
                        TipoEnum = rol.TipoEnum,
                        CantidadRepresentantes = rol.DelegadosClub.Count
                    };

                    return Ok(rolDto);
                }
                else
                {
                    // Si no es un enum válido, buscar como string
                    var rol = await _context.Roles
                        .Where(r => r.Tipo.ToLower() == tipo.ToLower())
                        .Select(r => new RolDto
                        {
                            IdRol = r.IdRol,
                            Tipo = r.Tipo,
                            TipoEnum = r.TipoEnum,
                            CantidadRepresentantes = r.DelegadosClub.Count
                        })
                        .FirstOrDefaultAsync();

                    if (rol == null)
                    {
                        return NotFound($"Rol con tipo '{tipo}' no encontrado");
                    }

                    return Ok(rol);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Rol/predefinidos
        [HttpGet("predefinidos")]
        public async Task<ActionResult<IEnumerable<RolDto>>> GetRolesPredefinidos()
        {
            try
            {
                // 🔹 Usar el enum para definir roles predefinidos
                var rolesPredefinidos = new List<RolTipo>
                {
                    RolTipo.Administrador,
                    RolTipo.DelegadoClub,
                    RolTipo.Entrenador,
                    RolTipo.Atleta,
                    // Agrega otros si es necesario
                };

                // Usar extensión para obtener múltiples roles
                var roles = await _context.Roles.GetByTiposAsync(rolesPredefinidos.ToArray());

                var rolesDto = roles.Select(r => new RolDto
                {
                    IdRol = r.IdRol,
                    Tipo = r.Tipo,
                    TipoEnum = r.TipoEnum,
                    CantidadRepresentantes = r.DelegadosClub.Count
                }).ToList();

                return Ok(rolesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // 🔹 NUEVO ENDPOINT: Obtener valores del enum
        // GET: api/Rol/enum-values
        [HttpGet("enum-values")]
        public ActionResult GetEnumValues()
        {
            try
            {
                var enumValues = Enum.GetValues(typeof(RolTipo))
                    .Cast<RolTipo>()
                    .Select(e => new
                    {
                        Id = (int)e,
                        Nombre = e.ToString(),
                        Descripcion = GetRoleDescription(e)
                    })
                    .ToList();

                return Ok(enumValues);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // 🔹 NUEVO ENDPOINT: Obtener rol por ID de enum
        // GET: api/Rol/enum-id/1
        [HttpGet("enum-id/{enumId}")]
        public async Task<ActionResult<RolDto>> GetRolPorEnumId(int enumId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(RolTipo), enumId))
                {
                    return BadRequest($"ID de enum inválido: {enumId}. IDs válidos: 1-{Enum.GetValues(typeof(RolTipo)).Length}");
                }

                var rolTipo = (RolTipo)enumId;
                var rol = await _context.Roles.GetByTipoAsync(rolTipo);

                if (rol == null)
                {
                    return NotFound($"Rol con enum ID {enumId} ({rolTipo}) no encontrado");
                }

                var rolDto = new RolDto
                {
                    IdRol = rol.IdRol,
                    Tipo = rol.Tipo,
                    TipoEnum = rol.TipoEnum,
                    CantidadRepresentantes = rol.DelegadosClub.Count
                };

                return Ok(rolDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.IdRol == id);
        }

        private string GetRoleDescription(RolTipo tipo)
        {
            return tipo switch
            {
                RolTipo.Administrador => "Acceso total al sistema",
                RolTipo.PresidenteFederacion => "Máxima autoridad de una federación",
                RolTipo.DelegadoClub => "Representante oficial de un club",
                RolTipo.Entrenador => "Entrenador de club",
                RolTipo.EntrenadorSeleccion => "Entrenador de selección nacional",
                RolTipo.Atleta => "Deportista registrado",
                RolTipo.Secretario => "Personal administrativo",
                _ => "Sin descripción"
            };
        }
    }
}