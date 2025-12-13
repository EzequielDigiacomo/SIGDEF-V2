using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Club
{
    public class ClubCreateDto
    {
        [Required(ErrorMessage = "El nombre del club es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Las siglas son requeridas")]
        [MaxLength(10, ErrorMessage = "Las siglas no pueden exceder 10 caracteres")]
        public string Siglas { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

}
