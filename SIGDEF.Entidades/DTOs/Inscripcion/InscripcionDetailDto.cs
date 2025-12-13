using SIGDEF.DTOs;
using SIGDEF.Entidades.DTOs.Atleta;
using SIGDEF.Entidades.DTOs.Evento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Inscripcion
{
    public class InscripcionDetailDto
    {
        public int IdInscripcion { get; set; }
        public int IdAtleta { get; set; }
        public int IdEvento { get; set; }
        public int IdEventoPrueba { get; set; }
        public DateTime FechaInscripcion { get; set; }

        public AtletaDto? Atleta { get; set; }
        public EventoDto? Evento { get; set; }
    }
}
