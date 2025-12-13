using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Usuario
{
    public class UsuarioResponseDto
    {
        public int IdUsuario { get; set; }
        public int? IdPersona { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool EstaActivo { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpira { get; set; }

        // Información de persona (opcional)
        public string? NombreCompleto { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; } = string.Empty;
        public int? IdClub { get; set; }
    }
}
