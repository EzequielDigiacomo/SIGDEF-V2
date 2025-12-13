using SIGDEF.DTOs;
using SIGDEF.Entidades.DTOs.Tutor;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.Enums;

namespace SIGDEF.Entidades.DTOs.AtletaTutor
{
    public class AtletaTutorDetailDto
    {
        public int IdAtleta { get; set; }
        public int IdTutor { get; set; }
        public Parentesco Parentesco { get; set; }
        public AtletaDto? Atleta { get; set; }
        public TutorDto? Tutor { get; set; }
    }
}