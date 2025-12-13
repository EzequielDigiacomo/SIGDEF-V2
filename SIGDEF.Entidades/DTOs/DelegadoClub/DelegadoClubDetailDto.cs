using SIGDEF.Entidades.DTOs.Club;
using SIGDEF.Entidades.DTOs.Federacion;
using SIGDEF.Entidades.DTOs.Persona;
using SIGDEF.Entidades.DTOs.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.DelegadoClub
{
    public class DelegadoClubDetailDto
    {
        public int IdPersona { get; set; }
        public int IdRol { get; set; }
        public int IdFederacion { get; set; }
        public int IdClub { get; set; }

        public PersonaDto? Persona { get; set; }
        public RolDto? Rol { get; set; }
        public FederacionDto? Federacion { get; set; }
        public ClubDto? Club { get; set; }
    }
}
