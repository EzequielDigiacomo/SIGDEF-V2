using SIGDEF.Entidades.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIGDEF.Entidades.DTOs.EventoPrueba
{
    public class EventoPruebaCreateDto
    {
       public int IdEvento { get; set; }
        [Required(ErrorMessage = "La distancia es requerida")]
        [EnumDataType(typeof(DistanciaRegata), ErrorMessage = "Distancia no válida")]
        public DistanciaRegata Distancia { get; set; }
        
        [Required]
        public SexoCompetencia SexoCompetencia { get; set; }
        [Required]
        public TipoBote TipoBote { get; set; }
        [Required]
        public CategoriaEdad CategoriaEdad { get; set; }
        
        public decimal? PrecioCategoria { get; set; }
    }
}
