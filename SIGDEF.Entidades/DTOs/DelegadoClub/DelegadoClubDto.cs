using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.DelegadoClub
{
    public class DelegadoClubDto
    {
        public int IdPersona { get; set; }
        public int IdRol { get; set; }
        public int IdFederacion { get; set; }
        public int IdClub { get; set; }

        // Información adicional para mostrar
        public string? NombrePersona { get; set; }
        public string? TipoRol { get; set; }
        public string? NombreFederacion { get; set; }
        public string? NombreClub { get; set; }
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }
}
