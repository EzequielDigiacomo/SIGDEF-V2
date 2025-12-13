using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGDEF.Entidades.DTOs.Inscripcion;

namespace SIGDEF.Entidades.DTOs.Evento
{
    public class EventoDetailDto
    {
        public int IdEvento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        // Información relacionada existente
        public List<InscripcionDto>? Inscripciones { get; set; }
        // NUEVO: Agrega esta lista para el Cronograma
        public List<EventoPruebaDto> Pruebas { get; set; } = new List<EventoPruebaDto>();
    }
}
