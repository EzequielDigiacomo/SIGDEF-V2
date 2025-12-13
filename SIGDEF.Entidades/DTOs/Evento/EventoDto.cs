using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Evento
{
    public class EventoDto
    {
        public int IdEvento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // Estadísticas para mostrar en listas
        public int CantidadInscripciones { get; set; }
        public int TotalAtletas { get; set; }      // ← NUEVO
        public int TotalClubes { get; set; }       // ← NUEVO
        public string Estado { get; set; } = string.Empty;

        public List<EventoPruebaDto> Pruebas { get; set; } = new List<EventoPruebaDto>();
    }
}
