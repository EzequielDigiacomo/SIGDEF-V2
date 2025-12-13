// 📄 SIGDEF/Helpers/CategoriaEdadExtensions.cs
using SIGDEF.Entidades.Enums;

namespace SIGDEF.Helpers
{
    public static class CategoriaEdadExtensions
    {
        public static string ToDisplayString(this CategoriaEdad categoria)
        {
            return categoria switch
            {
                CategoriaEdad.Preinfantil => "Pre-Infantil",
                CategoriaEdad.Infantil => "Infantil",
                CategoriaEdad.Cadete => "Cadete",
                CategoriaEdad.Junior => "Junior",
                CategoriaEdad.Sub21 => "Sub-21",
                CategoriaEdad.Sub23 => "Sub-23",
                CategoriaEdad.Senior => "Senior",
                CategoriaEdad.MasterA => "Master",
                _ => categoria.ToString()
            };
        }

        public static string ToCodigo(this CategoriaEdad categoria)
        {
            return categoria switch
            {
                CategoriaEdad.Preinfantil => "PRE",
                CategoriaEdad.Infantil => "INF",
                CategoriaEdad.Cadete => "CAD",
                CategoriaEdad.Junior => "JUN",
                CategoriaEdad.Sub21 => "S21",
                CategoriaEdad.Sub23 => "S23",
                CategoriaEdad.Senior => "SEN",
                CategoriaEdad.MasterA => "MAS",
                _ => categoria.ToString().Substring(0, 3).ToUpper()
            };
        }

        public static (int? min, int? max) GetRangoEdad(this CategoriaEdad categoria)
        {
            return categoria switch
            {
                CategoriaEdad.Preinfantil => (6, 9),
                CategoriaEdad.Infantil => (10, 12),
                CategoriaEdad.Cadete => (13, 14),
                CategoriaEdad.Junior => (15, 17),
                CategoriaEdad.Sub21 => (18, 20),
                CategoriaEdad.Sub23 => (21, 22),
                CategoriaEdad.Senior => (23, 39),
                CategoriaEdad.MasterA => (40, null), // Sin límite superior
                _ => (null, null)
            };
        }

        public static int? GetEdadMinima(this CategoriaEdad categoria)
        {
            var (min, _) = categoria.GetRangoEdad();
            return min;
        }

        public static int? GetEdadMaxima(this CategoriaEdad categoria)
        {
            var (_, max) = categoria.GetRangoEdad();
            return max;
        }

        public static string GetSexoDefault(this CategoriaEdad categoria)
        {
            // Por defecto todas son mixtas, pero puedes personalizar
            return "Mixto";
        }

        public static string GetDescripcion(this CategoriaEdad categoria)
        {
            var (min, max) = categoria.GetRangoEdad();

            if (min.HasValue && max.HasValue)
                return $"{categoria.ToDisplayString()} ({min}-{max} años)";
            else if (min.HasValue)
                return $"{categoria.ToDisplayString()} ({min}+ años)";
            else
                return categoria.ToDisplayString();
        }

        public static CategoriaEdad GetCategoriaPorEdad(int edad)
        {
            return edad switch
            {
                >= 40 => CategoriaEdad.MasterA,
                >= 23 => CategoriaEdad.Senior,
                >= 21 => CategoriaEdad.Sub23,
                >= 18 => CategoriaEdad.Sub21,
                >= 15 => CategoriaEdad.Junior,
                >= 13 => CategoriaEdad.Cadete,
                >= 10 => CategoriaEdad.Infantil,
                >= 6 => CategoriaEdad.Preinfantil,
                _ => throw new ArgumentException($"Edad {edad} no válida para categorías")
            };
        }
    }
}