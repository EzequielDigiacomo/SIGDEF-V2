using SIGDEF.DTOs;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.DelegadoClub;
using SIGDEF.Entidades.DTOs.Entrenador;
using SIGDEF.Entidades.DTOs.PagoTransaccion;
using SIGDEF.Entidades.DTOs.Tutor;
using SIGDEF.Entidades.DTOs.Usuario;
using SIGDEF.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Persona
{
    public class PersonaDetailDto
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public Sexo Sexo { get; set; } // NUEVO
        public string SexoDisplay { get; set; } = string.Empty; // Para mostrar en UI
        // Información relacionada
        public UsuarioDto? Usuario { get; set; }
        public DelegadoClubDto? DelegadoClub { get; set; }
        public EntrenadorDto? Entrenador { get; set; }
        public TutorDto? Tutor { get; set; }
        public AtletaDto? Atleta { get; set; }
        public List<PagoTransaccionDto>? Pagos { get; set; }
    }
}
