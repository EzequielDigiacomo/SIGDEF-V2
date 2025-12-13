using SIGDEF.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.PagoTransaccion
{
    public class PagoTransaccionCreateDto
    {
        [Required(ErrorMessage = "El concepto es requerido")]
        [MaxLength(100, ErrorMessage = "El concepto no puede exceder 100 caracteres")]
        public string Concepto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public EstadoPagoTransaccion Estado { get; set; }

        [Required(ErrorMessage = "El ID de la persona es requerido")]
        public int IdPersona { get; set; }

        [Required(ErrorMessage = "El ID del club es requerido")]
        public int IdClub { get; set; }

        [MaxLength(100, ErrorMessage = "El ID de MercadoPago no puede exceder 100 caracteres")]
        public string IdMercadoPago { get; set; } = string.Empty;
    }

    
}
