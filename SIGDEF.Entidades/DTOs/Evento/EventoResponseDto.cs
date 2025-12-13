using System;
using System.Collections.Generic;
using System.Linq;

namespace SIGDEF.Entidades.DTOs.Evento
{
    public class EventoResponseDto
    {
        public int IdEvento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        // 🔹 TIPO DE EVENTO
        public int TipoEventoId { get; set; }
        public string TipoEventoNombre { get; set; } = string.Empty;
        public string TipoEventoIcono { get; set; } = string.Empty;
        public string TipoEventoColor { get; set; } = string.Empty;

        // 🔹 FECHAS
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime? FechaInicioInscripciones { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }

        // 🔹 UBICACIÓN
        public string? Ubicacion { get; set; }
        public string? Ciudad { get; set; }
        public string? Provincia { get; set; }

        // 🔹 PRUEBAS (LISTA DE CARRERAS)
        public List<EventoPruebaDto> Pruebas { get; set; } = new List<EventoPruebaDto>();

        // 🔹 PROPIEDADES PARA COMPATIBILIDAD (primera prueba)
        public int DistanciaId => Pruebas.FirstOrDefault()?.DistanciaId ?? 0;
        public string DistanciaCodigo => Pruebas.FirstOrDefault()?.DistanciaCodigo ?? string.Empty;
        public string DistanciaNombre => Pruebas.FirstOrDefault()?.DistanciaNombre ?? string.Empty;
        public decimal DistanciaMetros => Pruebas.FirstOrDefault()?.Metros ?? 0;
        public string DistanciasDisplay => string.Join(", ", Pruebas.Select(d => d.DistanciaCodigo));

        // 🔹 CONFIGURACIÓN
        public decimal PrecioBase { get; set; }
        public int CupoMaximo { get; set; }
        public bool TieneCronometraje { get; set; }
        public bool RequiereCertificadoMedico { get; set; }

        // 🔹 ESTADO
        public bool EstaActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Observaciones { get; set; }

        // 🔹 ESTADÍSTICAS
        public int TotalInscritos { get; set; }
        public int CuposDisponibles { get; set; }
        public bool InscripcionesAbiertas { get; set; }
        public bool TieneCupoDisponible { get; set; }
        public int DiasRestantes { get; set; }

        // 🔹 PROPIEDADES CALCULADAS PARA FRONTEND
        public string FechasDisplay { get; set; } = string.Empty;
        public string PeriodoInscripcionesDisplay { get; set; } = string.Empty;
        public string EstadoDisplay { get; set; } = string.Empty;
        public string UbicacionCompleta { get; set; } = string.Empty;
        public string PrecioDisplay { get; set; } = string.Empty;
        public string CupoDisplay { get; set; } = string.Empty;

        // 🔹 MÉTODO PRINCIPAL ACTUALIZADO
        public static EventoResponseDto FromEntity(Entidades.Evento evento)
        {
            if (evento == null)
                return null;

            var dto = new EventoResponseDto
            {
                // 🔹 INFORMACIÓN BÁSICA
                IdEvento = evento.IdEvento,
                Nombre = evento.Nombre,
                Descripcion = evento.Descripcion,

                // 🔹 TIPO DE EVENTO
                TipoEventoId = (int)evento.TipoEvento,
                TipoEventoNombre = GetTipoEventoDisplay(evento.TipoEvento),
                TipoEventoIcono = GetTipoEventoIcono(evento.TipoEvento),
                TipoEventoColor = GetTipoEventoColor(evento.TipoEvento),

                // 🔹 FECHAS
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                FechaInicioInscripciones = evento.FechaInicioInscripciones,
                FechaFinInscripciones = evento.FechaFinInscripciones,

                // 🔹 UBICACIÓN
                Ubicacion = evento.Ubicacion,
                Ciudad = evento.Ciudad,
                Provincia = evento.Provincia,

                // 🔹 PRUEBAS (MÚLTIPLES)
                Pruebas = evento.Pruebas?.Select(ed => EventoPruebaDto.FromEntity(ed)).ToList() ?? new List<EventoPruebaDto>(),

                // 🔹 CONFIGURACIÓN
                PrecioBase = evento.PrecioBase,
                CupoMaximo = evento.CupoMaximo,
                TieneCronometraje = evento.TieneCronometraje,
                RequiereCertificadoMedico = evento.RequiereCertificadoMedico,

                // 🔹 ESTADO
                EstaActivo = evento.EstaActivo,
                FechaCreacion = evento.FechaCreacion,
                Observaciones = evento.Observaciones,

                // 🔹 ESTADÍSTICAS
                TotalInscritos = evento.Inscripciones?.Count ?? 0,
                CuposDisponibles = evento.CupoMaximo - (evento.Inscripciones?.Count ?? 0),
                DiasRestantes = (evento.FechaInicio - DateTime.UtcNow).Days
            };

            // 🔹 CALCULAR PROPIEDADES DERIVADAS
            dto.InscripcionesAbiertas = CalcularInscripcionesAbiertas(evento);
            dto.TieneCupoDisponible = dto.CuposDisponibles > 0;

            // 🔹 STRINGS FORMATEADOS PARA FRONTEND
            dto.FechasDisplay = $"{dto.FechaInicio:dd/MM/yyyy} - {dto.FechaFin:dd/MM/yyyy}";

            dto.PeriodoInscripcionesDisplay = dto.FechaInicioInscripciones.HasValue && dto.FechaFinInscripciones.HasValue
                ? $"{dto.FechaInicioInscripciones:dd/MM/yyyy} - {dto.FechaFinInscripciones:dd/MM/yyyy}"
                : "Sin período definido";

            dto.EstadoDisplay = dto.EstaActivo
                ? (dto.InscripcionesAbiertas ? "🟢 Inscripciones Abiertas" : "🟡 Próximamente")
                : "🔴 Finalizado";

            dto.UbicacionCompleta = string.Join(", ",
                new[] { dto.Ubicacion, dto.Ciudad, dto.Provincia }
                    .Where(x => !string.IsNullOrEmpty(x)));

            dto.PrecioDisplay = dto.PrecioBase > 0
                ? $"${dto.PrecioBase:N2}"
                : "Gratuito";

            dto.CupoDisplay = $"{dto.TotalInscritos}/{dto.CupoMaximo} inscritos";

            return dto;
        }

        // Métodos auxiliares para TipoEvento (mantener igual)
        private static string GetTipoEventoDisplay(Entidades.Enums.TipoEvento tipo)
        {
            return tipo switch
            {
                Entidades.Enums.TipoEvento.CarreraOficial => "Carrera Oficial",
                Entidades.Enums.TipoEvento.Campeonato => "Campeonato",
                Entidades.Enums.TipoEvento.Recreativo => "Recreativo",
                Entidades.Enums.TipoEvento.Entrenamiento => "Entrenamiento",
                Entidades.Enums.TipoEvento.Clasificatorio => "Clasificatorio",
                _ => tipo.ToString()
            };
        }

        private static string GetTipoEventoIcono(Entidades.Enums.TipoEvento tipo)
        {
            return tipo switch
            {
                Entidades.Enums.TipoEvento.CarreraOficial => "🏁",
                Entidades.Enums.TipoEvento.Campeonato => "🏆",
                Entidades.Enums.TipoEvento.Recreativo => "🎯",
                Entidades.Enums.TipoEvento.Entrenamiento => "⚽",
                Entidades.Enums.TipoEvento.Clasificatorio => "📊",
                _ => "📅"
            };
        }

        private static string GetTipoEventoColor(Entidades.Enums.TipoEvento tipo)
        {
            return tipo switch
            {
                Entidades.Enums.TipoEvento.CarreraOficial => "#FF6B6B",
                Entidades.Enums.TipoEvento.Campeonato => "#4ECDC4",
                Entidades.Enums.TipoEvento.Recreativo => "#45B7D1",
                Entidades.Enums.TipoEvento.Entrenamiento => "#96CEB4",
                Entidades.Enums.TipoEvento.Clasificatorio => "#FECA57",
                _ => "#C8D6E5"
            };
        }

        private static bool CalcularInscripcionesAbiertas(Entidades.Evento evento)
        {
            var ahora = DateTime.UtcNow;
            var inicioOk = !evento.FechaInicioInscripciones.HasValue || ahora >= evento.FechaInicioInscripciones;
            var finOk = !evento.FechaFinInscripciones.HasValue || ahora <= evento.FechaFinInscripciones;
            return inicioOk && finOk && evento.EstaActivo;
        }
    }

    // 🔹 DTO PARA PRUEBAS DEL EVENTO
    public class EventoPruebaDto
    {
        public int IdEventoPrueba { get; set; }
        public int DistanciaId { get; set; }
        public string DistanciaCodigo { get; set; } = string.Empty;
        public string DistanciaNombre { get; set; } = string.Empty;
        public decimal Metros { get; set; }
        public int CategoriaEdad { get; set; }
        public decimal? PrecioCategoria { get; set; }
        public int DistanciaRegata { get; set; } // Enum DistanciaRegata (1, 2, 3...)
        public int TipoBote { get; set; }  // Enum TipoBote (0=K1...)
        public string TipoBoteNombre { get; set; } = string.Empty;
        public int SexoCompetencia { get; set; }      // Enum SexoCompetencia

        public static EventoPruebaDto FromEntity(Entidades.EventoPrueba eventoPrueba)
        {
            return new EventoPruebaDto
            {
                IdEventoPrueba = eventoPrueba.IdEventoPrueba,
                DistanciaId = (int)eventoPrueba.Distancia,
                DistanciaCodigo = GetDistanciaDisplay(eventoPrueba.Distancia),
                DistanciaNombre = eventoPrueba.Distancia.ToString(),
                Metros = GetDistanciaMetros(eventoPrueba.Distancia),
                CategoriaEdad = (int)eventoPrueba.CategoriaEdad,
                PrecioCategoria = eventoPrueba.PrecioCategoria,
                TipoBote = (int)eventoPrueba.TipoBote,
                TipoBoteNombre = eventoPrueba.TipoBote.ToString(),
                SexoCompetencia = (int)eventoPrueba.SexoCompetencia,
                DistanciaRegata = (int)eventoPrueba.Distancia

            };
        }

        private static string GetDistanciaDisplay(Entidades.Enums.DistanciaRegata distancia)
        {
            return distancia switch
            {
                Entidades.Enums.DistanciaRegata.DoscientosMetros => "200m",
                Entidades.Enums.DistanciaRegata.TrecientosCincuentaMetros => "350m",
                Entidades.Enums.DistanciaRegata.QuatroCientosMetros => "400m",
                Entidades.Enums.DistanciaRegata.QuinientosMetros => "500m",
                Entidades.Enums.DistanciaRegata.MilMetros => "1000m",
                Entidades.Enums.DistanciaRegata.DosKilometros => "2K",
                Entidades.Enums.DistanciaRegata.TresKilometros => "3K",
                Entidades.Enums.DistanciaRegata.CincoKilometros => "5K",
                Entidades.Enums.DistanciaRegata.DiezKilometros => "10K",
                Entidades.Enums.DistanciaRegata.QuinceKilometros => "15K",
                Entidades.Enums.DistanciaRegata.VeintiDosKilometros => "22K",
                Entidades.Enums.DistanciaRegata.VeintiCincoKilometros => "25K",
                Entidades.Enums.DistanciaRegata.TreintaDosKilometros => "32K",
                _ => distancia.ToString()
            };
        }

        private static decimal GetDistanciaMetros(Entidades.Enums.DistanciaRegata distancia)
        {
            return distancia switch
            {
                Entidades.Enums.DistanciaRegata.DoscientosMetros => 200,
                Entidades.Enums.DistanciaRegata.TrecientosCincuentaMetros => 350,
                Entidades.Enums.DistanciaRegata.QuatroCientosMetros => 400,
                Entidades.Enums.DistanciaRegata.QuinientosMetros => 500,
                Entidades.Enums.DistanciaRegata.MilMetros => 1000,
                Entidades.Enums.DistanciaRegata.DosKilometros => 2000,
                Entidades.Enums.DistanciaRegata.TresKilometros => 3000,
                Entidades.Enums.DistanciaRegata.CincoKilometros => 5000,
                Entidades.Enums.DistanciaRegata.DiezKilometros => 10000,
                Entidades.Enums.DistanciaRegata.QuinceKilometros => 15000,
                Entidades.Enums.DistanciaRegata.VeintiDosKilometros => 22000,
                Entidades.Enums.DistanciaRegata.VeintiCincoKilometros => 25000,
                Entidades.Enums.DistanciaRegata.TreintaDosKilometros => 32000,
                _ => 0
            };
        }
    }
}