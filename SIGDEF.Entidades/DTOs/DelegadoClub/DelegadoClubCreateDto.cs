using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.DelegadoClub
{
    public class DelegadoClubCreateDto
    {
        [Required(ErrorMessage = "El ID de la persona es requerido")]
        public int IdPersona { get; set; }

        [Required(ErrorMessage = "El ID del rol es requerido")]
        public int IdRol { get; set; }

        [Required(ErrorMessage = "El ID de la federación es requerido")]
        public int IdFederacion { get; set; }

        [Required(ErrorMessage = "El ID del club es requerido")]
        public int IdClub { get; set; }
    }

}
