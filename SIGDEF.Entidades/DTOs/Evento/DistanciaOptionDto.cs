using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Evento
{
    public class DistanciaOptionDto
    {
        public int IdDistanciaEnum { get; set; }                    
        public string CodigoDistanca { get; set; } = string.Empty;     // "200m", "5K", "10K"
        public string NombreDistancias { get; set; } = string.Empty;     // "Doscientos Metros"
        public decimal Metros { get; set; }             // 200, 5000, 10000
        public string TipoDistancia { get; set; } = string.Empty;       // "Sprint", "Media Distancia"
    }
}
