// 📄 SIGDEF/Entidades/DTOs/TipoEventoOptionDto.cs
using System.ComponentModel.DataAnnotations.Schema;
using SIGDEF.Entidades.Enums;
using SIGDEF.Helpers;

namespace SIGDEF.Entidades.DTOs
{
    public class TipoEventoOptionDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool RequiereInscripcion { get; set; }
        public bool PermiteMultipleDistancias { get; set; }
        public bool RequiereJueces { get; set; }
        public bool EsCompetitivo { get; set; }
        public bool EsFormativo { get; set; }
        public string NivelDificultad { get; set; } = string.Empty;
        public bool EsPopular { get; set; }

        public static TipoEventoOptionDto FromEnum(TipoEvento tipo)
        {
            return new TipoEventoOptionDto
            {
                Id = (int)tipo,
                Codigo = tipo.ToCodigo(),
                Nombre = tipo.ToDisplayString(),
                Icono = tipo.GetIcono(),
                Color = tipo.GetColor(),
                Descripcion = tipo.GetDescripcion(),
                RequiereInscripcion = tipo.RequiereInscripcion(),
                PermiteMultipleDistancias = tipo.PermiteMultipleDistancias(),
                RequiereJueces = tipo.RequiereJueces(),
                EsCompetitivo = tipo.EsCompetitivo(),
                EsFormativo = tipo.EsFormativo(),
                NivelDificultad = tipo.GetNivelDificultad(),
                EsPopular = tipo == TipoEvento.CarreraOficial ||
                           tipo == TipoEvento.Campeonato
            };
        }
    }
}