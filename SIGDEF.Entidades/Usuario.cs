using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }  // ← NUEVA PK

        public int? IdPersona { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool EstaActivo { get; set; } = true;
        
        [Required, MaxLength(50)]
        public string Rol { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime UltimoAcceso { get; set; } = DateTime.UtcNow;

        public int? IdClub { get; set; }

        [ForeignKey(nameof(IdClub))]
        public virtual Club? Club { get; set; }

        public virtual Persona Persona { get; set; } = null!;
    }
}
