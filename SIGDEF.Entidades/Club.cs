using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades
{
    public class Club
    {
        [Key]
        public int IdClub { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string Siglas { get; set; } = string.Empty;

        [Column("Email")]  // ← AGREGAR ESTA LÍNEA
        public string Email { get; set; } = string.Empty;

        public virtual ICollection<Atleta> Atletas { get; set; } = new List<Atleta>();
        public virtual ICollection<Entrenador> Entrenadores { get; set; } = new List<Entrenador>();
        public virtual ICollection<PagoTransaccion> Pagos { get; set; } = new List<PagoTransaccion>();
        public virtual ICollection<DelegadoClub> Representantes { get; set; } = new List<DelegadoClub>();
    }
}
