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
    public class AtletaTutor
    {
        [Key, Column(Order = 0)]
        public int IdAtleta { get; set; }

        [Key, Column(Order = 1)]
        public int IdTutor { get; set; }

        [ForeignKey(nameof(IdAtleta))]
        public virtual Atleta Atleta { get; set; } = null!;

        [ForeignKey(nameof(IdTutor))]
        public virtual Tutor Tutor { get; set; } = null!;

        public Parentesco Parentesco { get; set; }
    }
}
