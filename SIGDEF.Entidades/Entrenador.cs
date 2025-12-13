using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades
{
    public class Entrenador
    {
        [Key]
        [ForeignKey(nameof(Persona))]
        public int IdPersona { get; set; }

        public virtual Persona Persona { get; set; } = null!;

        [ForeignKey(nameof(Club))]
        public int IdClub { get; set; }
        public virtual Club Club { get; set; } = null!;

        [MaxLength(50)]
        public string Licencia { get; set; } = string.Empty;

        public bool PerteneceSeleccion { get; set; }
        [MaxLength(50)]
        public string CategoriaSeleccion { get; set; } = string.Empty;

        public bool BecadoEnard { get; set; }
        public bool BecadoSdn { get; set; }
        public decimal MontoBeca { get; set; }
        public bool PresentoAptoMedico { get; set; }
    }
}
