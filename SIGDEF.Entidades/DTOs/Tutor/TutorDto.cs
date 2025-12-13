using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Tutor
{
    public class TutorDto
    {
        public int IdPersona { get; set; }
        public string TipoTutor { get; set; } = string.Empty;

        // Información adicional para mostrar
        public string? NombrePersona { get; set; }
        public string? Documento { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int CantidadAtletas { get; set; }
    }
}
