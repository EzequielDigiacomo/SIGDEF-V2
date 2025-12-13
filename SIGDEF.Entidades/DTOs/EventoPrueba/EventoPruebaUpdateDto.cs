using System.ComponentModel.DataAnnotations;

namespace SIGDEF.Entidades.DTOs.EventoPrueba
{
    public class EventoPruebaUpdateDto : EventoPruebaCreateDto
    {
        [Required]
        public int IdEventoPrueba { get; set; }
    }
}
