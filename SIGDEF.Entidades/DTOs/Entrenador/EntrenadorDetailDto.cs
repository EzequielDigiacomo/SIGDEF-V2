using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.DTOs.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Entrenador
{
    public class EntrenadorDetailDto
    {
        public int IdPersona { get; set; }
        public int IdClub { get; set; }
        public string Licencia { get; set; } = string.Empty;
        public bool PerteneceSeleccion { get; set; }
        public string CategoriaSeleccion { get; set; } = string.Empty;
        public bool BecadoEnard { get; set; }
        public bool BecadoSdn { get; set; }
        public decimal MontoBeca { get; set; }
        public bool PresentoAptoMedico { get; set; }

        public PersonaDto? Persona { get; set; }
        public ClubDto? Club { get; set; }
    }
}
