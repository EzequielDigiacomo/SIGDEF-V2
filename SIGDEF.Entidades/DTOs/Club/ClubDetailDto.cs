using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.Entrenador;
using SIGDEF.Entidades.DTOs.DelegadoClub;
using SIGDEF.Entidades.DTOs.PagoTransaccion;

namespace SIGDEF.Entidades.DTOs.Club
{
    public class ClubDetailDto
    {
        public int IdClub { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Información relacionada
        public List<AtletaDto>? Atletas { get; set; }
        public List<EntrenadorDto>? Entrenadores { get; set; }
        public List<DelegadoClubDto>? Representantes { get; set; }
        public List<PagoTransaccionDto>? Pagos { get; set; }
    }
}
