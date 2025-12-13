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
    public class DelegadoClub
    {
        [Key] // 👈 ¡Obligatorio!
        [ForeignKey(nameof(Persona))]
        public int IdPersona { get; set; }

        public virtual Persona Persona { get; set; } = null!;

        [ForeignKey(nameof(Rol))]
        public int IdRol { get; set; }
        public virtual Rol Rol { get; set; } = null!;

        [ForeignKey(nameof(Federacion))]
        public int IdFederacion { get; set; }
        public virtual Federacion Federacion { get; set; } = null!;

        [ForeignKey(nameof(Club))]
        public int ClubIdClub { get; set; }
        public virtual Club Club { get; set; } = null!;
    }
}
