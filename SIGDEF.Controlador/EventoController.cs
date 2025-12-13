using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIGDEF.AccesoDatos;
using SIGDEF.DTOs;
using SIGDEF.Entidades;
using SIGDEF.Entidades.DTOs;
using SIGDEF.Entidades.DTOs.Evento;
using SIGDEF.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIGDEF.Entidades.DTOs.EventoPrueba;

namespace SIGDEF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly SIGDeFContext _context;

        public EventoController(SIGDeFContext context)
        {
            _context = context;
        }

        // GET: api/Eventos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetEventos(
    [FromQuery] bool? activos = null,
    [FromQuery] string? tipo = null,
    [FromQuery] string? provincia = null,
    [FromQuery] int? distancia = null)
        {
            // Usar proyección directamente
            var result = await _context.Eventos
                .Include(e => e.Pruebas)
                .Where(e =>
                    (!activos.HasValue || e.EstaActivo == activos.Value) &&
                    (string.IsNullOrEmpty(tipo) || e.TipoEvento.ToString() == tipo) &&
                    (string.IsNullOrEmpty(provincia) || e.Provincia == provincia) &&
                    (!distancia.HasValue || e.Pruebas.Any(ed => ed.Distancia == (DistanciaRegata)distancia.Value))
                )
                .Select(e => new EventoDto
                {
                    IdEvento = e.IdEvento,
                    Nombre = e.Nombre,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    CantidadInscripciones = e.Inscripciones.Count,
                    TotalAtletas = e.Inscripciones.Select(i => i.IdAtleta).Distinct().Count(),

                    // Obtener clubes únicos a través de Atleta -> Club
                    TotalClubes = e.Inscripciones
                        .Select(i => i.Atleta.IdClub) // Atleta debe tener ClubId
                        .Distinct()
                        .Count(),

                    Estado = e.EstaActivo
                        ? (e.FechaInicio > DateTime.UtcNow ? "Programado"
                           : e.FechaFin < DateTime.UtcNow ? "Finalizado" : "En curso")
                        : "Inactivo"
                })
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/Eventos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventoResponseDto>> GetEvento(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.Pruebas)
                .Include(e => e.Inscripciones)
                    .ThenInclude(i => i.Atleta)
                .Include(e => e.Inscripciones)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {id} no encontrado" });
            }

            // Usar el método FromEntity que ya tienes en EventoResponseDto
            return EventoResponseDto.FromEntity(evento);
        }

        // GET: api/Eventos/5/detalle
        [HttpGet("{id}/detalle")]
        public async Task<ActionResult<EventoDetailDto>> GetEventoDetalle(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.Pruebas)
                .Include(e => e.Inscripciones)
                    .ThenInclude(i => i.Atleta)
                .Include(e => e.Inscripciones)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {id} no encontrado" });
            }

            var dto = new EventoDetailDto
            {
                IdEvento = evento.IdEvento,
                Nombre = evento.Nombre,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                // Mapear inscripciones si es necesario
            };

            return dto;
        }

        // POST: api/Eventos
        [HttpPost]
        public async Task<ActionResult<EventoResponseDto>> PostEvento(EventoCreateDTO eventoDto)
        {
            // Validación de fechas
            if (eventoDto.FechaInicio >= eventoDto.FechaFin)
            {
                return BadRequest(new { message = "La fecha de inicio debe ser anterior a la fecha de fin" });
            }

            if (eventoDto.FechaInicioInscripciones.HasValue && eventoDto.FechaFinInscripciones.HasValue)
            {
                if (eventoDto.FechaInicioInscripciones >= eventoDto.FechaFinInscripciones)
                {
                    return BadRequest(new { message = "La fecha de inicio de inscripciones debe ser anterior a la fecha de fin" });
                }

                if (eventoDto.FechaFinInscripciones > eventoDto.FechaInicio)
                {
                    return BadRequest(new { message = "Las inscripciones no pueden cerrar después del inicio del evento" });
                }
            }

            // Validar distancias
            if (eventoDto.Distancias == null || !eventoDto.Distancias.Any())
            {
                return BadRequest(new { message = "El evento debe tener al menos una distancia" });
            }

            // Crear entidad
            var evento = new Evento
            {
                // Información básica
                Nombre = eventoDto.Nombre,
                Descripcion = eventoDto.Descripcion,
                TipoEvento = eventoDto.TipoEvento,

                // Fechas
                FechaInicio = eventoDto.FechaInicio,
                FechaFin = eventoDto.FechaFin,
                FechaInicioInscripciones = eventoDto.FechaInicioInscripciones,
                FechaFinInscripciones = eventoDto.FechaFinInscripciones,

                // Ubicación
                Ubicacion = eventoDto.Ubicacion,
                Ciudad = eventoDto.Ciudad,
                Provincia = eventoDto.Provincia,

                // Crear Pruebas desde el DTO
                Pruebas = eventoDto.Distancias.Select(d => new EventoPrueba
                {
                    Distancia = d.DistanciaRegata, // Asegúrate de usar la propiedad correcta del DTO
                    CategoriaEdad = (CategoriaEdad)d.CategoriaEdad, // Casteo de int a Enum
                    TipoBote = (TipoBote)d.TipoBote,            // Casteo de int a Enum (NUEVO)
                    SexoCompetencia = (SexoCompetencia)d.SexoCompetencia              // Casteo de int a Enum (NUEVO)
                }).ToList(),
                // Configuración
                PrecioBase = eventoDto.PrecioBase,
                CupoMaximo = eventoDto.CupoMaximo,
                TieneCronometraje = eventoDto.TieneCronometraje,
                RequiereCertificadoMedico = eventoDto.RequiereCertificadoMedico,

                // Observaciones
                Observaciones = eventoDto.Observaciones,

                // Auditoría
                FechaCreacion = DateTime.UtcNow,
                EstaActivo = true
            };

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            // Cargar relaciones para la respuesta
            await _context.Entry(evento)
                .Collection(e => e.Inscripciones)
                .LoadAsync();

            return CreatedAtAction(nameof(GetEvento),
                new { id = evento.IdEvento },
                EventoResponseDto.FromEntity(evento));
        }

        // PUT: api/Eventos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, EventoUpdateDto eventoDto)
        {
            if (id != eventoDto.IdEvento)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del evento" });
            }

            var evento = await _context.Eventos
                .Include(e => e.Pruebas)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {id} no encontrado" });
            }

            // Validación de fechas
            if (eventoDto.FechaInicio >= eventoDto.FechaFin)
            {
                return BadRequest(new { message = "La fecha de inicio debe ser anterior a la fecha de fin" });
            }

            // Actualizar propiedades básicas
            evento.Nombre = eventoDto.Nombre;
            evento.Descripcion = eventoDto.Descripcion;
            evento.TipoEvento = eventoDto.TipoEvento;
            evento.FechaInicio = eventoDto.FechaInicio;
            evento.FechaFin = eventoDto.FechaFin;
            evento.FechaInicioInscripciones = eventoDto.FechaInicioInscripciones;
            evento.FechaFinInscripciones = eventoDto.FechaFinInscripciones;
            evento.Ubicacion = eventoDto.Ubicacion;
            evento.Ciudad = eventoDto.Ciudad;
            evento.Provincia = eventoDto.Provincia;
            evento.PrecioBase = eventoDto.PrecioBase;
            evento.CupoMaximo = eventoDto.CupoMaximo;
            evento.TieneCronometraje = eventoDto.TieneCronometraje;
            evento.RequiereCertificadoMedico = eventoDto.RequiereCertificadoMedico;
            evento.Observaciones = eventoDto.Observaciones;
            evento.EstaActivo = eventoDto.EstaActivo;
            evento.FechaActualizacion = DateTime.UtcNow;

            // Actualizar distancias (eliminar todas y crear nuevas)
            if (eventoDto.Distancias != null && eventoDto.Distancias.Any())
            {
                // Eliminar pruebas existentes
                _context.EventoPruebas.RemoveRange(evento.Pruebas);

                // Crear nuevas pruebas
                evento.Pruebas = eventoDto.Distancias.Select(d => new EventoPrueba
                {
                    Distancia = d.DistanciaRegata,
                    CategoriaEdad = (CategoriaEdad)d.CategoriaEdad,
                    TipoBote = (TipoBote)d.TipoBote,
                    SexoCompetencia = (SexoCompetencia)d.SexoCompetencia
                }).ToList();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PATCH: api/Eventos/5/activar
        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> ActivarEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {id} no encontrado" });
            }

            evento.EstaActivo = true;
            evento.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Eventos/5/desactivar
        [HttpPatch("{id}/desactivar")]
        public async Task<IActionResult> DesactivarEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {id} no encontrado" });
            }

            evento.EstaActivo = false;
            evento.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Eventos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.Inscripciones)
                .Include(e => e.Pruebas)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {id} no encontrado" });
            }

            // Verificar si tiene inscripciones
            if (evento.Inscripciones.Any())
            {
                return BadRequest(new
                {
                    message = "No se puede eliminar un evento con inscripciones activas",
                    inscripcionesCount = evento.Inscripciones.Count
                });
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Eventos/proximos
        [HttpGet("proximos")]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetProximosEventos()
        {
            var hoy = DateTime.UtcNow;

            // Primero obtener eventos básicos
            var eventos = await _context.Eventos
                .Include(e => e.Pruebas)
                .Where(e => e.EstaActivo && e.FechaInicio > hoy)
                .OrderBy(e => e.FechaInicio)
                .Take(10)
                .ToListAsync();

            // Obtener IDs de eventos
            var eventoIds = eventos.Select(e => e.IdEvento).ToList();

            // Consulta separada para estadísticas (más eficiente)
            var estadisticas = await _context.Inscripciones
                .Where(i => eventoIds.Contains(i.IdEventoPrueba))
                .Include(i => i.Atleta) // Incluir Atleta aquí
                .Select(i => new
                {
                    i.IdEventoPrueba,
                    i.IdAtleta,
                    ClubId = i.Atleta.IdClub // o i.Atleta.IdClub
                })
                .ToListAsync();

            // Agrupar en memoria
            var statsPorEvento = estadisticas
                .GroupBy(i => i.IdEventoPrueba)
                .Select(g => new
                {
                    EventoId = g.Key,
                    TotalAtletas = g.Select(i => i.IdAtleta).Distinct().Count(),
                    TotalClubes = g.Select(i => i.ClubId).Distinct().Count(),
                    CantidadInscripciones = g.Count()
                })
                .ToDictionary(x => x.EventoId);

            var result = eventos.Select(e =>
            {
                var stats = statsPorEvento.GetValueOrDefault(e.IdEvento);

                return new EventoDto
                {
                    IdEvento = e.IdEvento,
                    Nombre = e.Nombre,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    CantidadInscripciones = stats?.CantidadInscripciones ?? 0,
                    TotalAtletas = stats?.TotalAtletas ?? 0,
                    TotalClubes = stats?.TotalClubes ?? 0,
                    Estado = "Programado"
                };
            }).ToList();

            return Ok(result);
        }

        // GET: api/Eventos/inscripciones-abiertas
        [HttpGet("inscripciones-abiertas")]
        public async Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventosConInscripcionesAbiertas()
        {
            var eventos = await _context.Eventos
                .Include(e => e.Pruebas)
                .Include(e => e.Inscripciones)
                .Where(e => e.EstaActivo && e.PuedeInscribirse())
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();

            var result = eventos.Select(EventoResponseDto.FromEntity).ToList();
            return Ok(result);
        }

        // GET: api/Eventos/por-distancia/3
        [HttpGet("por-distancia/{distancia}")]
        public async Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventosPorDistancia(int distancia)
        {
            var distanciaEnum = (DistanciaRegata)distancia;

            var eventos = await _context.Eventos
                .Include(e => e.Pruebas)
                .Include(e => e.Inscripciones)
                .Where(e => e.EstaActivo && e.Pruebas.Any(ed => ed.Distancia == distanciaEnum))
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();

            var result = eventos.Select(EventoResponseDto.FromEntity).ToList();
            return Ok(result);
        }

        // GET: api/Eventos/form-config
        [HttpGet("form-config")]
        public async Task<ActionResult<EventoFormConfigDto>> GetFormConfig()
        {
            var config = new EventoFormConfigDto
            {
                // Distancias disponibles desde el enum
                DistanciasDisponibles = Enum.GetValues(typeof(DistanciaRegata))
                    .Cast<DistanciaRegata>()
                    .Select(d => new DistanciaOptionDto
                    {
                        IdDistanciaEnum = (int)d,
                        CodigoDistanca = GetDistanciaCodigo(d),
                        NombreDistancias = d.ToString(),
                        Metros = GetDistanciaEnMetros(d),
                        TipoDistancia = GetTipoDistancia(d)
                    }).ToList(),

                // Categorías disponibles
                CategoriasDisponibles = Enum.GetValues(typeof(CategoriaEdad))
                    .Cast<CategoriaEdad>()
                    .Select(CategoriaOptionDto.FromEnum)
                    .ToList(),

                // Tipos de evento
                TiposEvento = Enum.GetValues(typeof(TipoEvento))
                    .Cast<TipoEvento>()
                    .Select(TipoEventoOptionDto.FromEnum)
                    .ToList(),

                // Tipos de bote
                TiposBote = Enum.GetValues(typeof(TipoBote))
                    .Cast<TipoBote>()
                    .Select(TipoBoteOptionDto.FromEnum)
                    .ToList()
            };

            return Ok(config);
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.IdEvento == id);
        }

        // Métodos auxiliares para DistanciaOptionDto
        private string GetDistanciaCodigo(DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => "200m",
                DistanciaRegata.TrecientosCincuentaMetros => "350m",
                DistanciaRegata.QuatroCientosMetros => "400m",
                DistanciaRegata.QuinientosMetros => "500m",
                DistanciaRegata.MilMetros => "1000m",
                DistanciaRegata.DosKilometros => "2K",
                DistanciaRegata.TresKilometros => "3K",
                DistanciaRegata.CincoKilometros => "5K",
                DistanciaRegata.DiezKilometros => "10K",
                DistanciaRegata.QuinceKilometros => "15K",
                DistanciaRegata.VeintiDosKilometros => "22K",
                DistanciaRegata.VeintiCincoKilometros => "25K",
                DistanciaRegata.TreintaDosKilometros => "32K",
                _ => distancia.ToString()
            };
        }

        private decimal GetDistanciaEnMetros(DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => 200,
                DistanciaRegata.TrecientosCincuentaMetros => 350,
                DistanciaRegata.QuatroCientosMetros => 400,
                DistanciaRegata.QuinientosMetros => 500,
                DistanciaRegata.MilMetros => 1000,
                DistanciaRegata.DosKilometros => 2000,
                DistanciaRegata.TresKilometros => 3000,
                DistanciaRegata.CincoKilometros => 5000,
                DistanciaRegata.DiezKilometros => 10000,
                DistanciaRegata.QuinceKilometros => 15000,
                DistanciaRegata.VeintiDosKilometros => 22000,
                DistanciaRegata.VeintiCincoKilometros => 25000,
                DistanciaRegata.TreintaDosKilometros => 32000,
                _ => 0
            };
        }

        private string GetTipoDistancia(DistanciaRegata distancia)
        {
            var metros = GetDistanciaEnMetros(distancia);
            return metros switch
            {
                <= 500 => "Sprint",
                <= 2000 => "Media Distancia",
                _ => "Larga Distancia"
            };
        }
    }
}