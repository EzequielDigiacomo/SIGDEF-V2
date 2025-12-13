// 📄 SIGDEF/Entidades/DTOs/CategoriaOptionDto.cs
using SIGDEF.Entidades.Enums;
using SIGDEF.Helpers;

namespace SIGDEF.Entidades.DTOs
{
    public class CategoriaOptionDto
    {
        public int IdCategoria { get; set; }
        public string CodigoCategoria { get; set; } = string.Empty;
        public string NombreCategoria { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        // Método estático para crear desde el enum
        public static CategoriaOptionDto FromEnum(CategoriaEdad categoria)
        {
            var (min, max) = categoria.GetRangoEdad();

            return new CategoriaOptionDto
            {
                IdCategoria = (int)categoria,
                CodigoCategoria = categoria.ToCodigo(),
                NombreCategoria = categoria.ToDisplayString(),
                Sexo = categoria.GetSexoDefault(),
                Descripcion = categoria.GetDescripcion()
            };
        }
    }
}