using SIGDEF.Entidades.Enums;
using System;

namespace SIGDEF.Entidades.DTOs.Evento
{
    public class TipoBoteOptionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;

        public static TipoBoteOptionDto FromEnum(TipoBote bote)
        {
            return new TipoBoteOptionDto
            {
                Id = (int)bote,
                Nombre = bote.ToString(), 
                Codigo = bote.ToString()
            };
        }
    }
}
