using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades
{
    public class Federacion
    {
        [Key]
        public int IdFederacion { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Cuit { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BancoNombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TipoCuenta { get; set; } = string.Empty;

        [MaxLength(50)]
        public string NumeroCuenta { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TitularCuenta { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmailCobro { get; set; } = string.Empty;

        public virtual ICollection<DelegadoClub> DelegadosClub { get; set; } = new List<DelegadoClub>();
    }
}
