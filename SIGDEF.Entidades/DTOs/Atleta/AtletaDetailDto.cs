using SIGDEF.Entidades.Enums;
using System;
using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.DTOs.Persona;
using SIGDEF.Entidades.DTOs.Inscripcion;
using SIGDEF.Entidades.DTOs.AtletaTutor;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Atleta
{
    public class AtletaDetailDto
    {
        public int IdPersona { get; set; }
        public int IdClub { get; set; }
        public EstadoPago EstadoPago { get; set; }
        public bool PerteneceSeleccion { get; set; }
        public CategoriaEdad? Categoria { get; set; }
        public bool BecadoEnard { get; set; }
        public bool BecadoSdn { get; set; }
        public decimal MontoBeca { get; set; }
        public bool PresentoAptoMedico { get; set; }
        public DateTime? FechaAptoMedico { get; set; }
         public DateTime FechaCreacion { get; set; }
        // Información relacionada
        public PersonaDto? Persona { get; set; }
        public ClubDto? Club { get; set; }
        public List<InscripcionDto>? Inscripciones { get; set; }
        public List<AtletaTutorDto>? Tutores { get; set; }
    }
}
