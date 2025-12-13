using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades
{
    public class Inscripcion
    {
        [Key]
        public int IdInscripcion { get; set; }

        [ForeignKey(nameof(Atleta))]
        public int IdAtleta { get; set; }
        public virtual Atleta Atleta { get; set; } = null!;

        [ForeignKey(nameof(EventoPrueba))]
        public int IdEventoPrueba { get; set; }
        public virtual EventoPrueba EventoPrueba { get; set; } = null!;

        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }
}

