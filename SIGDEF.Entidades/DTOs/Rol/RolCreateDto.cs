using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Rol
{
    public class RolCreateDto
    {
        [Required(ErrorMessage = "El tipo de rol es requerido")]
        [MaxLength(50, ErrorMessage = "El tipo de rol no puede exceder 50 caracteres")]
        public string Tipo { get; set; } = string.Empty;
    }

}
