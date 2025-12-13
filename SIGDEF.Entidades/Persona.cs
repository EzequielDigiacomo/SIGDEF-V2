using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGDEF.Entidades.Enums;

namespace SIGDEF.Entidades
{
    public class Persona
    {
        [Key]
        public int IdPersona { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Documento { get; set; } = string.Empty;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [MaxLength(100)]
        [Column("Email")]
        public string? Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Telefono { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Direccion { get; set; } = string.Empty;

        [Required]
        public Sexo Sexo { get; set; }

        // Relaciones (virtuales)
        public virtual Usuario? Usuario { get; set; }
        public virtual DelegadoClub? DelegadoClub { get; set; }
        public virtual Entrenador? Entrenador { get; set; }
        public virtual Tutor? Tutor { get; set; }
        public virtual Atleta? Atleta { get; set; }
        public virtual ICollection<PagoTransaccion> Pagos { get; set; } = new List<PagoTransaccion>();
        public virtual ICollection<DocumentacionPersona> Documentacion { get; set; } = new List<DocumentacionPersona>();
    }
}
