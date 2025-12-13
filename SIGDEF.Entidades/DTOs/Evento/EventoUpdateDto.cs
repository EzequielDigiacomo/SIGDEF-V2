// EventoActualizarDTO.cs (similar pero con Id)
using SIGDEF.Entidades.Enums;
using System.ComponentModel.DataAnnotations;

namespace SIGDEF.DTOs
{
        public class EventoUpdateDto : EventoCreateDTO
        {
            [Required(ErrorMessage = "El ID del evento es requerido")]
            public int IdEvento { get; set; }

            public bool EstaActivo { get; set; } = true;
         }

}