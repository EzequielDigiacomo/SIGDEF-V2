using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGDEF.AccesoDatos;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs.DelegadoClub;
using SIGDEF.Entidades.DTOs.Entrenador;
using SIGDEF.Entidades.DTOs.PagoTransaccion;
using SIGDEF.Entidades.DTOs.Persona;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.Tutor;
using SIGDEF.Entidades.DTOs.Usuario;
using SIGDEF.Entidades.Enums; // Asegúrate de incluir este using

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonaController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public PersonaController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Persona
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonaDto>>> GetPersonas()
        {
            try
            {
                var personas = await _context.Personas
                    .Select(p => new PersonaDto
                    {
                        IdPersona = p.IdPersona,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Documento = p.Documento,
                        FechaNacimiento = p.FechaNacimiento,
                        Email = p.Email,
                        Telefono = p.Telefono,
                        Direccion = p.Direccion,
                        Sexo = p.Sexo, // NUEVO
                        SexoDisplay = p.Sexo.ToString(), // NUEVO
                        Edad = CalcularEdad(p.FechaNacimiento),
                        NombreCompleto = p.Nombre + " " + p.Apellido,
                        TipoPersona = GetTipoPersona(p)
                    })
                    .ToListAsync();

                return Ok(personas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/Persona/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonaDetailDto>> GetPersona(int id)
        {
            try
            {
                var persona = await _context.Personas
                    .Include(p => p.Usuario)
                    .Include(p => p.DelegadoClub)
                    .Include(p => p.Entrenador)
                    .Include(p => p.Tutor)
                    .Include(p => p.Atleta)
                    .Include(p => p.Pagos)
                    .Where(p => p.IdPersona == id)
                    .Select(p => new PersonaDetailDto
                    {
                        IdPersona = p.IdPersona,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Documento = p.Documento,
                        FechaNacimiento = p.FechaNacimiento,
                        Email = p.Email,
                        Telefono = p.Telefono,
                        Direccion = p.Direccion,
                        Sexo = p.Sexo, // NUEVO
                        SexoDisplay = p.Sexo.ToString(), // NUEVO
                        Usuario = p.Usuario != null ? new UsuarioDto
                        {
                            IdPersona = p.Usuario.IdPersona,
                            Username = p.Usuario.Username,
                            EstaActivo = p.Usuario.EstaActivo,
                            FechaCreacion = p.Usuario.FechaCreacion,
                            UltimoAcceso = p.Usuario.UltimoAcceso
                        } : null,
                        DelegadoClub = p.DelegadoClub != null ? new DelegadoClubDto
                        {
                            IdPersona = p.DelegadoClub.IdPersona,
                            IdRol = p.DelegadoClub.IdRol,
                            IdFederacion = p.DelegadoClub.IdFederacion
                        } : null,
                        Entrenador = p.Entrenador != null ? new EntrenadorDto
                        {
                            IdPersona = p.Entrenador.IdPersona,
                            IdClub = p.Entrenador.IdClub,
                            Licencia = p.Entrenador.Licencia,
                            PerteneceSeleccion = p.Entrenador.PerteneceSeleccion,
                            CategoriaSeleccion = p.Entrenador.CategoriaSeleccion,
                            BecadoEnard = p.Entrenador.BecadoEnard,
                            BecadoSdn = p.Entrenador.BecadoSdn,
                            MontoBeca = p.Entrenador.MontoBeca,
                            PresentoAptoMedico = p.Entrenador.PresentoAptoMedico
                        } : null,
                        Tutor = p.Tutor != null ? new TutorDto
                        {
                            IdPersona = p.Tutor.IdPersona,
                            TipoTutor = p.Tutor.TipoTutor
                        } : null,
                        Atleta = p.Atleta != null ? new AtletaDto
                        {
                            IdPersona = p.Atleta.IdPersona,
                            IdClub = p.Atleta.IdClub,
                            EstadoPago = p.Atleta.EstadoPago,
                            PerteneceSeleccion = p.Atleta.PerteneceSeleccion,
                            Categoria = p.Atleta.Categoria,
                            BecadoEnard = p.Atleta.BecadoEnard,
                            BecadoSdn = p.Atleta.BecadoSdn,
                            MontoBeca = p.Atleta.MontoBeca,
                            PresentoAptoMedico = p.Atleta.PresentoAptoMedico,
                            FechaAptoMedico = p.Atleta.FechaAptoMedico
                        } : null,
                        Pagos = p.Pagos.Select(pa => new PagoTransaccionDto
                        {
                            IdPago = pa.IdPago,
                            Concepto = pa.Concepto,
                            Monto = pa.Monto,
                            Estado = pa.Estado,
                            FechaCreacion = pa.FechaCreacion,
                            FechaAprobacion = pa.FechaAprobacion,
                            IdPersona = pa.IdPersona,
                            IdClub = pa.IdClub,
                            IdMercadoPago = pa.IdMercadoPago
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (persona == null)
                {
                    return NotFound($"Persona con ID {id} no encontrada");
                }

                return Ok(persona);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/Persona
        [HttpPost]
        public async Task<ActionResult<PersonaDto>> PostPersona(PersonaCreateDto personaCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // 🔹 Validar que el documento no exista
                var documentoExists = await _context.Personas.AnyAsync(p => p.Documento == personaCreateDto.Documento);
                if (documentoExists)
                {
                    return BadRequest("Ya existe una persona con este documento");
                }

                // 🔹 Validar que el sexo sea válido
                if (!Enum.IsDefined(typeof(Sexo), personaCreateDto.Sexo))
                {
                    return BadRequest("Valor de sexo no válido. Valores permitidos: 1=Masculino, 2=Femenino, 3=Otro, 4=PrefieroNoDecir");
                }

                // 🔹 CONVERTIR FECHA A UTC (SOLUCIÓN AL ERROR DE POSTGRESQL)
                var fechaNacimientoUtc = DateTime.SpecifyKind(personaCreateDto.FechaNacimiento, DateTimeKind.Utc);

                // 🔹 Validar que la fecha de nacimiento sea válida (no futura)
                if (fechaNacimientoUtc > DateTime.UtcNow)
                {
                    return BadRequest("La fecha de nacimiento no puede ser futura");
                }

                // 🔹 Validar edad mínima (por ejemplo, 5 años)
                var edadMinima = DateTime.UtcNow.AddYears(-5);
                if (fechaNacimientoUtc > edadMinima)
                {
                    return BadRequest("La persona debe tener al menos 5 años");
                }

                var persona = new Persona
                {
                    Nombre = personaCreateDto.Nombre,
                    Apellido = personaCreateDto.Apellido,
                    Documento = personaCreateDto.Documento,
                    FechaNacimiento = fechaNacimientoUtc,
                    Email = personaCreateDto.Email ?? string.Empty,
                    Telefono = personaCreateDto.Telefono ?? string.Empty,
                    Direccion = personaCreateDto.Direccion ?? string.Empty,
                    Sexo = personaCreateDto.Sexo // NUEVO
                };

                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                var personaDto = new PersonaDto
                {
                    IdPersona = persona.IdPersona,
                    Nombre = persona.Nombre,
                    Apellido = persona.Apellido,
                    Documento = persona.Documento,
                    FechaNacimiento = persona.FechaNacimiento,
                    Email = persona.Email,
                    Telefono = persona.Telefono,
                    Direccion = persona.Direccion,
                    Sexo = persona.Sexo, // NUEVO
                    SexoDisplay = persona.Sexo.ToString(), // NUEVO
                    Edad = CalcularEdad(persona.FechaNacimiento),
                    NombreCompleto = persona.Nombre + " " + persona.Apellido,
                    TipoPersona = "Persona Base"
                };

                return CreatedAtAction(nameof(GetPersona), new { id = persona.IdPersona }, personaDto);
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

        // PUT: api/Persona/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona(int id, PersonaCreateDto personaCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var persona = await _context.Personas.FindAsync(id);
                if (persona == null)
                {
                    return NotFound($"Persona con ID {id} no encontrada");
                }

                // 🔹 Validar que el documento no exista en otra persona
                var documentoExists = await _context.Personas
                    .AnyAsync(p => p.Documento == personaCreateDto.Documento && p.IdPersona != id);
                if (documentoExists)
                {
                    return BadRequest("Ya existe otra persona con este documento");
                }

                // 🔹 Validar que el sexo sea válido
                if (!Enum.IsDefined(typeof(Sexo), personaCreateDto.Sexo))
                {
                    return BadRequest("Valor de sexo no válido. Valores permitidos: 1=Masculino, 2=Femenino, 3=Otro, 4=PrefieroNoDecir");
                }

                // 🔹 CONVERTIR FECHA A UTC (SOLUCIÓN AL ERROR DE POSTGRESQL)
                var fechaNacimientoUtc = DateTime.SpecifyKind(personaCreateDto.FechaNacimiento, DateTimeKind.Utc);

                // 🔹 Validar que la fecha de nacimiento sea válida
                if (fechaNacimientoUtc > DateTime.UtcNow)
                {
                    return BadRequest("La fecha de nacimiento no puede ser futura");
                }

                // Actualizar propiedades
                persona.Nombre = personaCreateDto.Nombre;
                persona.Apellido = personaCreateDto.Apellido;
                persona.Documento = personaCreateDto.Documento;
                persona.FechaNacimiento = fechaNacimientoUtc;
                persona.Email = personaCreateDto.Email ?? string.Empty;
                persona.Telefono = personaCreateDto.Telefono ?? string.Empty;
                persona.Direccion = personaCreateDto.Direccion ?? string.Empty;
                persona.Sexo = personaCreateDto.Sexo; // NUEVO

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(id))
                {
                    return NotFound($"Persona con ID {id} no existe");
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

        // ... (los métodos DELETE, Search, GetPersonaPorDocumento permanecen igual)

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.IdPersona == id);
        }

        private static int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        private static string GetTipoPersona(Persona persona)
        {
            if (persona.Atleta != null) return "Atleta";
            if (persona.Entrenador != null) return "Entrenador";
            if (persona.Tutor != null) return "Tutor";
            if (persona.DelegadoClub != null) return "DelegadoClub";
            if (persona.Usuario != null) return "Usuario";
            return "Persona Base";
        }
    }
}