using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SIGDEF.Entidades.Enums;

namespace SIGDEF.Entidades
{
    [Table("EventoPrueba")]
    public class EventoPrueba
    {
        [Key]
        public int IdEventoPrueba { get; set; }

        [Required]
        public int IdEvento { get; set; }

        [Required]
        [EnumDataType(typeof(DistanciaRegata))]
        public DistanciaRegata Distancia { get; set; }

        [Required]
        public CategoriaEdad CategoriaEdad { get; set; } // Enum CategoriaEdad

        public SexoCompetencia SexoCompetencia { get; set; }
        public TipoBote TipoBote { get; set; }

        public decimal? PrecioCategoria { get; set; } // Precio específico para esta prueba (opcional)

        [ForeignKey("IdEvento")]
        public virtual Evento Evento { get; set; } = null!;
    }
}
