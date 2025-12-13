using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.Enums;
using SIGDEF.Entidades.DTOs.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.PagoTransaccion
{
    public class PagoTransaccionDetailDto
    {
        public int IdPago { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public EstadoPagoTransaccion Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public int IdPersona { get; set; }
        public int IdClub { get; set; }
        public string IdMercadoPago { get; set; } = string.Empty;

        public PersonaDto? Persona { get; set; }
        public ClubDto? Club { get; set; }
    }
}
