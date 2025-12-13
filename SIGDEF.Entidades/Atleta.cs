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
    public class Atleta
    {
        [Key]
        public int IdPersona { get; set; }

        [ForeignKey(nameof(IdPersona))]
        public virtual Persona Persona { get; set; } = null!;

        [ForeignKey("Club")]
        public int IdClub { get; set; }
        public virtual Club? Club { get; set; } = null!;

        public EstadoPago EstadoPago { get; set; }

        public bool PerteneceSeleccion { get; set; }

        // 👇 Renombrado: CategoriaSeleccion → Categoria
        public CategoriaEdad? Categoria { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool BecadoEnard { get; set; }
        public bool BecadoSdn { get; set; }
        public decimal MontoBeca { get; set; }
        public bool PresentoAptoMedico { get; set; }
        public DateTime? FechaAptoMedico { get; set; }

        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
        public virtual ICollection<AtletaTutor> Tutores { get; set; } = new List<AtletaTutor>();
    }
}
