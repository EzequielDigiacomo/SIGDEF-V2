using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Inscripcion
{
    public class InscripcionDto
    {
        public int IdInscripcion { get; set; }
        public int IdAtleta { get; set; }
        public int IdEvento { get; set; }
        public int IdEventoPrueba { get; set; }
        public string? DetallePrueba { get; set; }
        public DateTime FechaInscripcion { get; set; }

        // Información adicional para mostrar
        public string? NombreAtleta { get; set; }
        public string? NombreEvento { get; set; }
        public string? NombreClub { get; set; }
        public DateTime? FechaInicioEvento { get; set; }
        public DateTime? FechaFinEvento { get; set; }
    }
}
