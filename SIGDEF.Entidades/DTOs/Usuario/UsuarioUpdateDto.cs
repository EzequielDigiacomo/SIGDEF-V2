using SIGDEF.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Usuario
{
    public class UsuarioUpdateDto
    {
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]

        public string? Username { get; set; }

        public bool? EstaActivo { get; set; }
        public RolTipo? Rol { get; set; }
        public int? IdClub { get; set; } // Agregar si quieres cambiar el club
    }
}
