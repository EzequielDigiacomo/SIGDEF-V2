using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Usuario
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public int? IdPersona { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool EstaActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimoAcceso { get; set; }

        // Información adicional para mostrar
        public string? NombrePersona { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; }

        public int? IdClub { get; set; }
    }
}
