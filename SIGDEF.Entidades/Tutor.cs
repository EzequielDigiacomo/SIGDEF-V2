using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades
{
    public class Tutor
    {
        [Key]
        [ForeignKey(nameof(Persona))]
        public int IdPersona { get; set; }

        public virtual Persona Persona { get; set; } = null!;

        [MaxLength(50)]
        public string TipoTutor { get; set; } = string.Empty;

        public virtual ICollection<AtletaTutor> AtletasTutores { get; set; } = new List<AtletaTutor>();
    }
}
