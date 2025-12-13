using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Tutor
{
    public class TutorCreateDto
    {
        [Required(ErrorMessage = "El ID de la persona es requerido")]
        public int IdPersona { get; set; }

        [Required(ErrorMessage = "El tipo de tutor es requerido")]
        [MaxLength(50, ErrorMessage = "El tipo de tutor no puede exceder 50 caracteres")]
        public string TipoTutor { get; set; } = string.Empty;
    }

}
