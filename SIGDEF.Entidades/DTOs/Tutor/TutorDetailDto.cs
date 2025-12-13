using SIGDEF.Entidades.DTOs.AtletaTutor;
using SIGDEF.Entidades.DTOs.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Tutor
{
    public class TutorDetailDto
    {
        public int IdPersona { get; set; }
        public string TipoTutor { get; set; } = string.Empty;

        public PersonaDto? Persona { get; set; }
        public List<AtletaTutorDto>? AtletasTutores { get; set; }
    }
}
