using SIGDEF.Entidades.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SIGDEF.DTOs
{
    public class AtletaCreateDto
    {
        [Required]
        public int IdPersona { get; set; }

        [Required]
        public int IdClub { get; set; }

        [Required]
        public EstadoPago EstadoPago { get; set; }

        [Required]
        public bool PerteneceSeleccion { get; set; }

        public CategoriaEdad? Categoria { get; set; }

        [Required]
        public bool BecadoEnard { get; set; }

        [Required]
        public bool BecadoSdn { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto de la beca debe ser mayor o igual a 0")]
        public decimal MontoBeca { get; set; }

        [Required]
        public bool PresentoAptoMedico { get; set; }

        public DateTime? FechaAptoMedico { get; set; }
        public DateTime FechaCreacion { get; set; }

        // 🔹 NUEVAS PROPIEDADES PARA INFORMACIÓN RELACIONADA
        public string? NombrePersona { get; set; }
        public string? NombreClub { get; set; }
    }
}