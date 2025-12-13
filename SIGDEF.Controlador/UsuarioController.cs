using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.Usuario;
using SIGDEF.Entidades.DTOs.Persona;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public UsuarioController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Include(u => u.Persona)
                    .Select(u => new UsuarioDto
                    {
                        IdUsuario = u.IdUsuario,
                        IdPersona = u.IdPersona,
                        Username = u.Username,
                        EstaActivo = u.EstaActivo,
                        FechaCreacion = u.FechaCreacion,
                        UltimoAcceso = u.UltimoAcceso,
                        NombrePersona = u.Persona.Nombre + " " + u.Persona.Apellido,
                        Email = u.Persona.Email,
                        Rol = "Usuario" // Puedes ajustar esto según tu lógica de roles
                    })
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDetailDto>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Persona)
                    .Where(u => u.IdPersona == id)
                    .Select(u => new UsuarioDetailDto
                    {
                        IdPersona = u.IdPersona,
                        Username = u.Username,
                        EstaActivo = u.EstaActivo,
                        FechaCreacion = u.FechaCreacion,
                        UltimoAcceso = u.UltimoAcceso,
                        Persona = new PersonaDto
                        {
                            IdPersona = u.Persona.IdPersona,
                            Nombre = u.Persona.Nombre,
                            Apellido = u.Persona.Apellido,
                            Documento = u.Persona.Documento,
                            FechaNacimiento = u.Persona.FechaNacimiento,
                            Email = u.Persona.Email,
                            Telefono = u.Persona.Telefono,
                            Direccion = u.Persona.Direccion
                        }
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> PostUsuario(UsuarioCreateDto usuarioCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que la persona existe
                var personaExists = await _context.Personas.AnyAsync(p => p.IdPersona == usuarioCreateDto.IdPersona);
                if (!personaExists)
                {
                    return BadRequest("La persona especificada no existe");
                }

                // 🔹 Validar que no sea usuario ya
                var usuarioExists = await _context.Usuarios.AnyAsync(u => u.IdPersona == usuarioCreateDto.IdPersona);
                if (usuarioExists)
                {
                    return BadRequest("Esta persona ya tiene un usuario registrado");
                }

                // 🔹 Validar que el username no exista
                var usernameExists = await _context.Usuarios.AnyAsync(u => u.Username == usuarioCreateDto.Username);
                if (usernameExists)
                {
                    return BadRequest("El nombre de usuario ya está en uso");
                }

                // 🔹 Hashear la contraseña
                var passwordHash = HashPassword(usuarioCreateDto.Password);

                var usuario = new Usuario
                {
                    IdPersona = usuarioCreateDto.IdPersona,
                    Username = usuarioCreateDto.Username,
                    PasswordHash = passwordHash,
                    EstaActivo = usuarioCreateDto.EstaActivo,
                    FechaCreacion = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // 🔹 Cargar datos relacionados para la respuesta
                await _context.Entry(usuario)
                    .Reference(u => u.Persona)
                    .LoadAsync();

                var usuarioDto = new UsuarioDto
                {
                    IdPersona = usuario.IdPersona,
                    Username = usuario.Username,
                    EstaActivo = usuario.EstaActivo,
                    FechaCreacion = usuario.FechaCreacion,
                    UltimoAcceso = usuario.UltimoAcceso,
                    NombrePersona = usuario.Persona.Nombre + " " + usuario.Persona.Apellido,
                    Email = usuario.Persona.Email,
                    Rol = "Usuario"
                };

                // 🔹 RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdPersona }, usuarioDto);
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

        // POST: api/Usuario/login
        // POST: api/Usuario/login
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> Login(UsuarioLoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuario = await _context.Usuarios
                    .Include(u => u.Persona)
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.EstaActivo);

                if (usuario == null)
                {
                    // 🔹 CORREGIDO: Usar Unauthorized sin parámetro o retornar un objeto
                    return BadRequest("Usuario o contraseña incorrectos"); 
                }

                // 🔹 Verificar contraseña
                if (!VerifyPassword(loginDto.Password, usuario.PasswordHash))
                {
                    // 🔹 CORREGIDO: Usar Unauthorized sin parámetro o retornar un objeto
                    return BadRequest("Usuario o contraseña incorrectos");
                }

                // 🔹 Actualizar último acceso
                usuario.UltimoAcceso = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var usuarioDto = new UsuarioDto
                {
                    IdPersona = usuario.IdPersona,
                    Username = usuario.Username,
                    EstaActivo = usuario.EstaActivo,
                    FechaCreacion = usuario.FechaCreacion,
                    UltimoAcceso = usuario.UltimoAcceso,
                    NombrePersona = usuario.Persona.Nombre + " " + usuario.Persona.Apellido,
                    Email = usuario.Persona.Email,
                    Rol = "Usuario"
                };

                return Ok(usuarioDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/Usuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateDto usuarioUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                // 🔹 Validar que el nuevo username no exista (si se está actualizando)
                if (!string.IsNullOrEmpty(usuarioUpdateDto.Username) && usuarioUpdateDto.Username != usuario.Username)
                {
                    var usernameExists = await _context.Usuarios.AnyAsync(u => u.Username == usuarioUpdateDto.Username && u.IdPersona != id);
                    if (usernameExists)
                    {
                        return BadRequest("El nombre de usuario ya está en uso");
                    }
                    usuario.Username = usuarioUpdateDto.Username;
                }

                // 🔹 Actualizar propiedades si se proporcionan
                if (usuarioUpdateDto.EstaActivo.HasValue)
                {
                    usuario.EstaActivo = usuarioUpdateDto.EstaActivo.Value;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound($"Usuario con ID {id} no existe");
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

        // PUT: api/Usuario/5/change-password
        [HttpPut("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, UsuarioChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                // 🔹 Verificar contraseña actual
                if (!VerifyPassword(changePasswordDto.CurrentPassword, usuario.PasswordHash))
                {
                    return BadRequest("La contraseña actual es incorrecta");
                }

                // 🔹 Hashear nueva contraseña
                usuario.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar el usuario: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Usuario/username/juanperez
        [HttpGet("username/{username}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuarioPorUsername(string username)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Persona)
                    .Where(u => u.Username == username)
                    .Select(u => new UsuarioDto
                    {
                        IdPersona = u.IdPersona,
                        Username = u.Username,
                        EstaActivo = u.EstaActivo,
                        FechaCreacion = u.FechaCreacion,
                        UltimoAcceso = u.UltimoAcceso,
                        NombrePersona = u.Persona.Nombre + " " + u.Persona.Apellido,
                        Email = u.Persona.Email,
                        Rol = "Usuario"
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound($"Usuario con username {username} no encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdPersona == id);
        }

        // 🔹 MÉTODOS PARA MANEJO DE CONTRASEÑAS
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}