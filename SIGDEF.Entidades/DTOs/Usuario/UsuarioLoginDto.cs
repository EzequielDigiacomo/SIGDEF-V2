using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Usuario
{
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
