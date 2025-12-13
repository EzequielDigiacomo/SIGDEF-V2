// EventoCrearDTO.cs
using SIGDEF.Entidades.DTOs.Evento;
using SIGDEF.Entidades.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SIGDEF.DTOs
{
    public class EventoCreateDTO
    {
        // 🔹 INFORMACIÓN BÁSICA
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        // 🔹 TIPO DE EVENTO
        [Required(ErrorMessage = "El tipo de evento es requerido")]
        [EnumDataType(typeof(TipoEvento), ErrorMessage = "Tipo de evento no válido")]
        public TipoEvento TipoEvento { get; set; } = TipoEvento.CarreraOficial;

        // 🔹 FECHAS
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFin { get; set; }

        // 🔹 FECHAS DE INSCRIPCIÓN
        public DateTime? FechaInicioInscripciones { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }

        // 🔹 UBICACIÓN
        [MaxLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        public string? Ubicacion { get; set; }

        [MaxLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
        public string? Ciudad { get; set; }

        [MaxLength(100, ErrorMessage = "La provincia no puede exceder 100 caracteres")]
        public string? Provincia { get; set; }

        // 🔹 DISTANCIA
        [Required(ErrorMessage = "Debe especificar al menos una distancia")]
        [MinLength(1, ErrorMessage = "Debe especificar al menos una distancia")]
        public List<DistanciaDTO> Distancias { get; set; } = new List<DistanciaDTO>();

        // 🔹 CONFIGURACIÓN
        [Range(0, 100000, ErrorMessage = "El precio debe estar entre 0 y 100,000")]
        public decimal PrecioBase { get; set; } = 0;

        [Range(1, 10000, ErrorMessage = "El cupo debe estar entre 1 y 10,000")]
        public int CupoMaximo { get; set; } = 100;

        public bool TieneCronometraje { get; set; } = true;
        public bool RequiereCertificadoMedico { get; set; } = false;

        // 🔹 OBSERVACIONES
        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observaciones { get; set; }
    }
}