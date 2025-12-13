using SIGDEF.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Persona
{
    public class PersonaCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [MaxLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El documento es requerido")]
        [MaxLength(50, ErrorMessage = "El documento no puede exceder 50 caracteres")]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime FechaNacimiento { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [MaxLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string? Email { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Telefono { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;

        public Sexo Sexo { get; set; } // NUEVO
        public string SexoDisplay { get; set; } = string.Empty; // Para mostrar en UI
    }

}
