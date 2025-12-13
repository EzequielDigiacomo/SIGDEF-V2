using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Inscripcion
{
    public class InscripcionCreateDto
    {
        [Required(ErrorMessage = "El ID del atleta es requerido")]
        public int IdAtleta { get; set; }

        [Required(ErrorMessage = "El ID de la prueba es requerido")]
        public int IdEventoPrueba { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    }
}
