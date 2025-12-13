using SIGDEF.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades
{
    public class PagoTransaccion
    {
        [Key]
        public int IdPago { get; set; }

        [Required, MaxLength(100)]
        public string Concepto { get; set; } = string.Empty;

        public decimal Monto { get; set; }

        public EstadoPagoTransaccion Estado { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaAprobacion { get; set; }

        [ForeignKey(nameof(Persona))]
        public int IdPersona { get; set; }
        public virtual Persona Persona { get; set; } = null!;

        [ForeignKey(nameof(Club))]
        public int IdClub { get; set; }
        public virtual Club Club { get; set; } = null!;

        [MaxLength(100)]
        public string IdMercadoPago { get; set; } = string.Empty;
    }
}
