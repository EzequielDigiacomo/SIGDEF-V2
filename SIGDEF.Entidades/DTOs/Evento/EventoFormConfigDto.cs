using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.DTOs.Evento
{
    public class EventoFormConfigDto
    {
        // 🔹 DISTANCIAS DISPONIBLES
        public List<DistanciaOptionDto> DistanciasDisponibles { get; set; } = new();

        // 🔹 CATEGORÍAS DISPONIBLES
        public List<CategoriaOptionDto> CategoriasDisponibles { get; set; } = new();

        // 🔹 TIPOS DE EVENTO
        public List<TipoEventoOptionDto> TiposEvento { get; set; } = new();

        // 🔹 TIPOS DE BOTE
        public List<TipoBoteOptionDto> TiposBote { get; set; } = new();

        // Método para obtener categorías sugeridas por distancia
        public List<CategoriaOptionDto> GetCategoriasSugeridasParaDistancia(int distanciaId)
        {
            // Lógica para sugerir categorías según la distancia
            // Ejemplo: para distancias cortas, todas las categorías
            // Para maratón, solo Senior y Master
            return CategoriasDisponibles
                .Where(c => c.IdCategoria >= 4) // Junior en adelante para distancias largas
                .ToList();
        }
    }
}
