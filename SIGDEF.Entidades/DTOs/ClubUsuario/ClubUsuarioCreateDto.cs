using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.CLubUsuario
{
    public class ClubUsuarioCreateDto
    {
        public int IdClub { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool EstaActivo { get; set; } = true;
        public string Rol { get; set; } = "Club";
    }
}
