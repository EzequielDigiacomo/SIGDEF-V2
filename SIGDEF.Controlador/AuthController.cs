using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.CLubUsuario;
using SIGDEF.Entidades.DTOs.Usuario;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SIGDEF.Controlador
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SIGDeFContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(SIGDeFContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 🔹 REGISTRO PARA USUARIOS PERSONA (Admin, Entrenador, Atleta, Usuario)
        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioResponseDto>> Registrar(UsuarioCreateDto usuarioCreateDto)
        {
            Debug.WriteLine("====== REGISTRO USUARIO PERSONA ======");
            Debug.WriteLine($"JSON recibido: {JsonSerializer.Serialize(usuarioCreateDto)}");
            Debug.WriteLine($"Username: {usuarioCreateDto.Username}");
            Debug.WriteLine($"IdPersona: {usuarioCreateDto.IdPersona}");
            Debug.WriteLine($"Rol solicitado: {usuarioCreateDto.Rol}");

            try
            {
                // 1. Validar contraseñas
                if (usuarioCreateDto.Password != usuarioCreateDto.ConfirmPassword)
                {
                    Debug.WriteLine("❌ ERROR: Las contraseñas no coinciden");
                    return BadRequest(new { message = "Las contraseñas no coinciden" });
                }

                // 2. Validar username único
                if (await _context.Usuarios.AnyAsync(u => u.Username == usuarioCreateDto.Username))
                {
                    Debug.WriteLine($"❌ ERROR: Username '{usuarioCreateDto.Username}' ya está en uso");
                    return BadRequest(new { message = "El nombre de usuario ya está en uso" });
                }

                // 3. Validar IdPersona (OBLIGATORIO para usuarios persona)
                if (usuarioCreateDto.IdPersona <= 0)
                {
                    Debug.WriteLine($"❌ ERROR: IdPersona inválido: {usuarioCreateDto.IdPersona}");
                    return BadRequest(new { message = "Debe especificar un IdPersona válido" });
                }

                // 4. Validar que la Persona existe
                var persona = await _context.Personas
                    .FirstOrDefaultAsync(p => p.IdPersona == usuarioCreateDto.IdPersona);

                if (persona == null)
                {
                    Debug.WriteLine($"❌ ERROR: Persona con Id {usuarioCreateDto.IdPersona} no existe");
                    return BadRequest(new { message = "La persona especificada no existe" });
                }

                // 5. Validar que la Persona no tenga usuario
                if (await _context.Usuarios.AnyAsync(u => u.IdPersona == usuarioCreateDto.IdPersona))
                {
                    Debug.WriteLine($"❌ ERROR: Persona {usuarioCreateDto.IdPersona} ya tiene usuario");
                    return BadRequest(new { message = "Esta persona ya tiene un usuario asociado" });
                }

                // 6. Validar rol (no puede ser "Club")
                if (usuarioCreateDto.Rol == "Club")
                {
                    Debug.WriteLine($"❌ ERROR: Rol 'Club' no permitido en este endpoint");
                    return BadRequest(new
                    {
                        message = "Para registrar un usuario Club use el endpoint /registrar-club"
                    });
                }

                // 7. Crear hash de contraseña
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(usuarioCreateDto.Password);
                Debug.WriteLine($"Password hash creado");

                // 8. Crear usuario PERSONA
                var usuario = new Usuario
                {
                    IdPersona = usuarioCreateDto.IdPersona,  // ✅ Obligatorio
                    IdClub = null,                           // ✅ Null para usuarios persona
                    Username = usuarioCreateDto.Username,
                    PasswordHash = passwordHash,
                    EstaActivo = usuarioCreateDto.EstaActivo,
                    FechaCreacion = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    Rol = usuarioCreateDto.Rol ?? "Usuario"   // Default "Usuario"
                };

                Debug.WriteLine($"Usuario a crear: IdPersona={usuario.IdPersona}, Rol={usuario.Rol}");

                // 9. Guardar en base de datos
                _context.Usuarios.Add(usuario);

                try
                {
                    await _context.SaveChangesAsync();
                    Debug.WriteLine("✅ Usuario persona guardado exitosamente");
                }
                catch (DbUpdateException dbEx)
                {
                    Debug.WriteLine($"💥 DB ERROR: {dbEx.InnerException?.Message}");
                    return StatusCode(500, new
                    {
                        message = "Error al guardar usuario",
                        error = dbEx.InnerException?.Message
                    });
                }

                // 10. Generar token
                var token = GenerarToken(usuario);
                Debug.WriteLine($"✅ Token generado exitosamente");

                // 11. Preparar respuesta
                return Ok(new UsuarioResponseDto
                {
                    IdPersona = usuario.IdPersona,
                    IdClub = usuario.IdClub,
                    Username = usuario.Username,
                    EstaActivo = usuario.EstaActivo,
                    UltimoAcceso = usuario.UltimoAcceso,
                    Token = token,
                    TokenExpira = DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                    NombreCompleto = $"{persona.Nombre} {persona.Apellido}",
                    Email = persona.Email,
                    Rol = usuario.Rol
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 ERROR GENERAL: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"💥 INNER: {ex.InnerException.Message}");
                }

                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        // 🔹 REGISTRO PARA USUARIOS CLUB
        [HttpPost("registrar-club")]
        public async Task<ActionResult<UsuarioResponseDto>> RegistrarClub(ClubUsuarioCreateDto clubCreateDto)
        {
            Debug.WriteLine("====== REGISTRO DE CLUB ======");
            Debug.WriteLine($"JSON recibido: {JsonSerializer.Serialize(clubCreateDto)}");

            try
            {
                // 1. Validar contraseñas
                if (clubCreateDto.Password != clubCreateDto.ConfirmPassword)
                {
                    Debug.WriteLine("❌ ERROR: Las contraseñas no coinciden");
                    return BadRequest(new { message = "Las contraseñas no coinciden" });
                }

                // 2. Validar username único
                if (await _context.Usuarios.AnyAsync(u => u.Username == clubCreateDto.Username))
                {
                    Debug.WriteLine($"❌ ERROR: Username '{clubCreateDto.Username}' ya está en uso");
                    return BadRequest(new { message = "El nombre de usuario ya está en uso" });
                }

                // 3. Validar IdClub (OBLIGATORIO para usuarios Club)
                if (clubCreateDto.IdClub <= 0)
                {
                    Debug.WriteLine($"❌ ERROR: IdClub inválido: {clubCreateDto.IdClub}");
                    return BadRequest(new { message = "Debe especificar un IdClub válido" });
                }

                // 4. Validar que el Club existe
                var club = await _context.Clubs
                    .FirstOrDefaultAsync(c => c.IdClub == clubCreateDto.IdClub);

                if (club == null)
                {
                    Debug.WriteLine($"❌ ERROR: Club con Id {clubCreateDto.IdClub} no existe");
                    return BadRequest(new
                    {
                        message = "El Club especificado no existe",
                        idClub = clubCreateDto.IdClub
                    });
                }

                // 5. Validar que el Club no tenga usuario
                if (await _context.Usuarios.AnyAsync(u => u.IdClub == clubCreateDto.IdClub))
                {
                    Debug.WriteLine($"❌ ERROR: Club {clubCreateDto.IdClub} ya tiene usuario");
                    return BadRequest(new
                    {
                        message = "Este Club ya tiene un usuario asociado",
                        idClub = clubCreateDto.IdClub
                    });
                }

                // 6. Crear hash de contraseña
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(clubCreateDto.Password);
                Debug.WriteLine($"Password hash creado");

                // 7. Crear usuario CLUB
                var usuario = new Usuario
                {
                    IdPersona = null,                  // ✅ Null para usuarios Club
                    IdClub = clubCreateDto.IdClub,     // ✅ Obligatorio
                    Username = clubCreateDto.Username,
                    PasswordHash = passwordHash,
                    EstaActivo = clubCreateDto.EstaActivo,
                    FechaCreacion = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                    Rol = "Club"                       // ✅ Siempre "Club"
                };

                Debug.WriteLine($"Usuario Club a crear: IdClub={usuario.IdClub}, Rol={usuario.Rol}");

                // 8. Guardar en base de datos
                _context.Usuarios.Add(usuario);

                try
                {
                    await _context.SaveChangesAsync();
                    Debug.WriteLine("✅ Usuario Club guardado exitosamente");
                }
                catch (DbUpdateException dbEx)
                {
                    Debug.WriteLine($"💥 DB ERROR: {dbEx.InnerException?.Message}");
                    return StatusCode(500, new
                    {
                        message = "Error al guardar usuario Club",
                        error = dbEx.InnerException?.Message
                    });
                }

                // 9. Generar token
                var token = GenerarToken(usuario);
                Debug.WriteLine($"✅ Token generado exitosamente");

                // 10. Preparar respuesta
                return Ok(new UsuarioResponseDto
                {
                    IdPersona = usuario.IdPersona,
                    IdClub = usuario.IdClub,
                    Username = usuario.Username,
                    EstaActivo = usuario.EstaActivo,
                    UltimoAcceso = usuario.UltimoAcceso,
                    Token = token,
                    TokenExpira = DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                    NombreCompleto = club.Nombre,
                    Email = club.Email,
                    Rol = "Club"
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 ERROR GENERAL: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"💥 INNER: {ex.InnerException.Message}");
                }

                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        // 🔹 LOGIN PARA AMBOS TIPOS DE USUARIOS
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioResponseDto>> Login(UsuarioLoginDto usuarioLoginDto)
        {
            try
            {
                Debug.WriteLine($"====== LOGIN ======");
                Debug.WriteLine($"Username: {usuarioLoginDto.Username}");

                // Buscar usuario incluyendo relaciones
                var usuario = await _context.Usuarios
                    .Include(u => u.Persona)
                    .Include(u => u.Club)
                    .FirstOrDefaultAsync(u => u.Username == usuarioLoginDto.Username);

                if (usuario == null || !usuario.EstaActivo)
                {
                    Debug.WriteLine($"❌ LOGIN ERROR: Usuario no encontrado o inactivo");
                    return StatusCode(401, new { message = "Usuario o contraseña incorrectos" });
                }

                // Verificar contraseña
                if (!BCrypt.Net.BCrypt.Verify(usuarioLoginDto.Password, usuario.PasswordHash))
                {
                    Debug.WriteLine($"❌ LOGIN ERROR: Contraseña incorrecta");
                    return StatusCode(401, new { message = "Usuario o contraseña incorrectos" });
                }

                // Actualizar último acceso
                usuario.UltimoAcceso = DateTime.Now;
                await _context.SaveChangesAsync();

                // Generar token
                var token = GenerarToken(usuario);
                Debug.WriteLine($"✅ LOGIN EXITOSO: Usuario={usuario.Username}, Rol={usuario.Rol}");

                // Preparar respuesta según tipo de usuario
                var response = new UsuarioResponseDto
                {
                    IdPersona = usuario.IdPersona,
                    IdClub = usuario.IdClub,
                    Username = usuario.Username,
                    EstaActivo = usuario.EstaActivo,
                    UltimoAcceso = usuario.UltimoAcceso,
                    Token = token,
                    TokenExpira = DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                    Rol = usuario.Rol ?? "Usuario"
                };

                // Agregar datos según tipo
                if (usuario.Rol == "Club" && usuario.Club != null)
                {
                    response.NombreCompleto = usuario.Club.Nombre;
                    response.Email = usuario.Club.Email;
                }
                else if (usuario.Persona != null)
                {
                    response.NombreCompleto = $"{usuario.Persona.Nombre} {usuario.Persona.Apellido}";
                    response.Email = usuario.Persona.Email;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 LOGIN ERROR: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // 🔹 GENERAR TOKEN JWT
        private string GenerarToken(Usuario usuario)
        {
            Debug.WriteLine($"🔐 Generando token para: {usuario.Username}, Rol={usuario.Rol}");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol ?? "Usuario")
            };

            // Agregar claims según tipo de usuario
            if (usuario.Rol == "Club" && usuario.IdClub.HasValue)
            {
                // Para usuarios Club
                claims.Add(new Claim("ClubId", usuario.IdClub.Value.ToString()));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.IdClub.Value.ToString()));

                var club = _context.Clubs.Find(usuario.IdClub.Value);
                if (club != null)
                {
                    claims.Add(new Claim(ClaimTypes.GivenName, club.Nombre));
                    if (!string.IsNullOrEmpty(club.Email))
                    {
                        claims.Add(new Claim(ClaimTypes.Email, club.Email));
                    }
                }
            }
            else if (usuario.IdPersona.HasValue)
            {
                // Para usuarios Persona
                claims.Add(new Claim("UsuarioId", usuario.IdPersona.Value.ToString()));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.IdPersona.Value.ToString()));

                var persona = _context.Personas.Find(usuario.IdPersona.Value);
                if (persona != null)
                {
                    claims.Add(new Claim(ClaimTypes.GivenName, $"{persona.Nombre} {persona.Apellido}"));
                    if (!string.IsNullOrEmpty(persona.Email))
                    {
                        claims.Add(new Claim(ClaimTypes.Email, persona.Email));
                    }
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // 🔹 MÉTODOS AUXILIARES (se mantienen igual)
        [HttpGet("verificar-usuario/{username}")]
        public async Task<ActionResult<bool>> VerificarUsuario(string username)
        {
            try
            {
                var existe = await _context.Usuarios
                    .AnyAsync(u => u.Username.ToLower() == username.ToLower());
                return Ok(existe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            try
            {
                var canConnect = _context.Database.CanConnect();
                return Ok(new
                {
                    status = canConnect ? "Healthy" : "Degraded",
                    timestamp = DateTime.UtcNow,
                    service = "SIGDEF API",
                    version = "1.0.0",
                    database = canConnect ? "Connected" : "Disconnected",
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    service = "SIGDEF API"
                });
            }
        }

        [HttpGet("perfil")]
        [Authorize]
        public async Task<ActionResult<UsuarioDetailDto>> ObtenerPerfil()
        {
            try
            {
                var rol = User.FindFirst(ClaimTypes.Role)?.Value;

                if (rol == "Club")
                {
                    // Perfil para Club
                    var clubIdClaim = User.FindFirst("ClubId")?.Value;
                    if (string.IsNullOrEmpty(clubIdClaim)) return Unauthorized();

                    var clubId = int.Parse(clubIdClaim);
                    var usuario = await _context.Usuarios
                        .Include(u => u.Club)
                        .FirstOrDefaultAsync(u => u.IdClub == clubId && u.Rol == "Club");

                    if (usuario == null) return Unauthorized();

                    return Ok(new UsuarioDetailDto
                    {
                        IdClub = usuario.IdClub,
                        Username = usuario.Username,
                        EstaActivo = usuario.EstaActivo,
                        FechaCreacion = usuario.FechaCreacion,
                        UltimoAcceso = usuario.UltimoAcceso,
                        Rol = usuario.Rol,
                        NombreClub = usuario.Club?.Nombre,
                        Emailclub = usuario.Club?.Email
                    });
                }
                else
                {
                    // Perfil para usuarios Persona
                    var usuarioIdClaim = User.FindFirst("UsuarioId")?.Value;
                    if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

                    var usuarioId = int.Parse(usuarioIdClaim);
                    var usuario = await _context.Usuarios
                        .Include(u => u.Persona)
                        .FirstOrDefaultAsync(u => u.IdPersona == usuarioId);

                    if (usuario == null) return Unauthorized();

                    return Ok(new UsuarioDetailDto
                    {
                        IdPersona = usuario.IdPersona,
                        Username = usuario.Username,
                        EstaActivo = usuario.EstaActivo,
                        FechaCreacion = usuario.FechaCreacion,
                        UltimoAcceso = usuario.UltimoAcceso,
                        Rol = usuario.Rol,
                        NombreClub = usuario.Persona != null ?
                            $"{usuario.Persona.Nombre} {usuario.Persona.Apellido}" : null,
                        Emailclub = usuario.Persona?.Email
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"💥 ERROR PERFIL: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("test-protegido")]
        [Authorize]
        public IActionResult TestProtegido()
        {
            var usuarioId = User.FindFirst("UsuarioId")?.Value;
            var clubId = User.FindFirst("ClubId")?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var nombreCompleto = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = "✅ Acceso concedido a endpoint protegido",
                usuarioId = usuarioId,
                clubId = clubId,
                username = username,
                nombreCompleto = nombreCompleto,
                email = email,
                rol = rol,
                hora = DateTime.Now,
                claims = User.Claims.Select(c => new
                {
                    tipo = c.Type,
                    valor = c.Value
                }).ToList()
            });
        }

        [HttpPost("cambiar-password")]
        [Authorize]
        public async Task<ActionResult> CambiarPassword(UsuarioChangePasswordDto changePasswordDto)
        {
            try
            {
                var rol = User.FindFirst(ClaimTypes.Role)?.Value;
                string? idClaim = null;

                if (rol == "Club")
                {
                    idClaim = User.FindFirst("ClubId")?.Value;
                }
                else
                {
                    idClaim = User.FindFirst("UsuarioId")?.Value;
                }

                if (string.IsNullOrEmpty(idClaim)) return Unauthorized();

                var id = int.Parse(idClaim);
                Usuario? usuario = null;

                if (rol == "Club")
                {
                    usuario = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.IdClub == id && u.Rol == "Club");
                }
                else
                {
                    usuario = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.IdPersona == id);
                }

                if (usuario == null) return Unauthorized();

                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, usuario.PasswordHash))
                {
                    return BadRequest(new { message = "La contraseña actual es incorrecta" });
                }

                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                {
                    return BadRequest(new { message = "Las nuevas contraseñas no coinciden" });
                }

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                usuario.UltimoAcceso = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Contraseña cambiada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("roles-disponibles")]
        [AllowAnonymous]
        public IActionResult GetRolesDisponibles()
        {
            var roles = new[]
            {
                new { Id = "Admin", Nombre = "Administrador", Descripcion = "Acceso total al sistema" },
                new { Id = "Club", Nombre = "Club Deportivo", Descripcion = "Gestión de club, atletas y entrenadores" },
                new { Id = "Entrenador", Nombre = "Entrenador", Descripcion = "Gestión de atletas y entrenamientos" },
                new { Id = "Atleta", Nombre = "Atleta", Descripcion = "Acceso a sus datos y eventos" },
                new { Id = "Usuario", Nombre = "Usuario Básico", Descripcion = "Acceso limitado" }
            };

            return Ok(roles);
        }
    }
}