using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Federacion
{
    public class FederacionDto
    {
        public int IdFederacion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string BancoNombre { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TitularCuenta { get; set; } = string.Empty;
        public string EmailCobro { get; set; } = string.Empty;
    }
}
