using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Entrenador
{
    public class EntrenadorDto
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

        // Información adicional para mostrar
        public string? NombrePersona { get; set; }
        public string? NombreClub { get; set; }
        public string? SiglasClub { get; set; }
    }
}
