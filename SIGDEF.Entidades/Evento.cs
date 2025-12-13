using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SIGDEF.Entidades.Enums;
using SIGDEF.Entidades.Extensions;
using SIGDEF.Helpers;

namespace SIGDEF.Entidades
{
    public class Evento
    {
        [Key]
        public int IdEvento { get; set; }

        // 🔹 INFORMACIÓN BÁSICA
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        // 🔹 TIPO DE EVENTO
        public TipoEvento TipoEvento { get; set; } = TipoEvento.CarreraOficial;

        // 🔹 FECHAS
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // 🔹 FECHAS DE INSCRIPCIÓN
        public DateTime? FechaInicioInscripciones { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }

        // 🔹 UBICACIÓN
        [MaxLength(200)]
        public string? Ubicacion { get; set; }

        [MaxLength(100)]
        public string? Ciudad { get; set; }

        [MaxLength(100)]
        public string? Provincia { get; set; }

        // 🔹 PRUEBAS (Carreras/Competencias dentro del evento)
        public virtual ICollection<EventoPrueba> Pruebas { get; set; } = new List<EventoPrueba>();

        // 🔹 CONFIGURACIÓN
        public decimal PrecioBase { get; set; } = 0;
        public int CupoMaximo { get; set; } = 100;
        public bool TieneCronometraje { get; set; } = true;
        public bool RequiereCertificadoMedico { get; set; } = false;

        // 🔹 ESTADO Y AUDITORÍA
        public bool EstaActivo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }

        [MaxLength(1000)]
        public string? Observaciones { get; set; }

        // 🔹 RELACIONES
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

        // 🔹 PROPIEDADES CALCULADAS
        [NotMapped]
        public string DistanciaDisplay => Pruebas != null && Pruebas.Any()
          ? string.Join(", ", Pruebas.Select(d => d.Distancia.ToDisplayString()).Distinct())
          : "Sin pruebas definidas";

        [NotMapped]
        public string TipoEventoDisplay => TipoEvento.ToDisplayString();

        [NotMapped]
        public string TipoEventoIcono => TipoEvento.GetIcono();

        [NotMapped]
        public string TipoEventoColor => TipoEvento.GetColor();

        [NotMapped]
        public string FechasDisplay => $"{FechaInicio:dd/MM/yyyy} - {FechaFin:dd/MM/yyyy}";

        [NotMapped]
        public int DiasRestantes => (FechaInicio - DateTime.UtcNow).Days;

        [NotMapped]
        public bool InscripcionesAbiertas =>
            (!FechaInicioInscripciones.HasValue || DateTime.UtcNow >= FechaInicioInscripciones) &&
            (!FechaFinInscripciones.HasValue || DateTime.UtcNow <= FechaFinInscripciones);

        [NotMapped]
        public int CuposDisponibles => CupoMaximo - Inscripciones.Count;

        [NotMapped]
        public bool TieneCupoDisponible => CuposDisponibles > 0;

        // 🔹 MÉTODOS
        public bool PuedeInscribirse()
        {
            return EstaActivo &&
                   InscripcionesAbiertas &&
                   TieneCupoDisponible;
                   /* &&PermiteInscripcionOnline*/;
        }

        public decimal CalcularPrecioFinal(DateTime fechaInscripcion)
        {
            decimal precio = PrecioBase;

            // Descuento por inscripción temprana
            if (FechaInicioInscripciones.HasValue &&
                fechaInscripcion < FechaInicioInscripciones.Value.AddDays(7))
            {
                precio *= 0.9m; // 10% descuento
            }

            return Math.Round(precio, 2);
        }
    }
}