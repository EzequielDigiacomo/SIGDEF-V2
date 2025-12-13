// 📄 SIGDEF/Helpers/TipoEventoExtensions.cs
using SIGDEF.Entidades.Enums;

namespace SIGDEF.Helpers
{
    public static class TipoEventoExtensions
    {
        public static string ToDisplayString(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => "Carrera Oficial",
                TipoEvento.Campeonato => "Campeonato",
                TipoEvento.Recreativo => "Recreativo",
                TipoEvento.Entrenamiento => "Entrenamiento",
                TipoEvento.Clasificatorio => "Clasificatorio",
                _ => tipo.ToString()
            };
        }

        public static string ToCodigo(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => "OFICIAL",
                TipoEvento.Campeonato => "CAMPEONATO",
                TipoEvento.Recreativo => "RECREATIVO",
                TipoEvento.Entrenamiento => "ENTRENAMIENTO",
                TipoEvento.Clasificatorio => "CLASIFICATORIO",
                _ => tipo.ToString().ToUpper()
            };
        }

        public static string GetIcono(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => "🏁",
                TipoEvento.Campeonato => "🏆",
                TipoEvento.Recreativo => "🎯",
                TipoEvento.Entrenamiento => "⚽",
                TipoEvento.Clasificatorio => "📊",
                _ => "📅"
            };
        }

        public static string GetColor(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => "#FF6B6B",     // Rojo
                TipoEvento.Campeonato => "#4ECDC4",         // Turquesa
                TipoEvento.Recreativo => "#45B7D1",         // Azul claro
                TipoEvento.Entrenamiento => "#96CEB4",      // Verde claro
                TipoEvento.Clasificatorio => "#FECA57",     // Amarillo
                _ => "#C8D6E5"                              // Gris claro
            };
        }

        public static string GetDescripcion(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => "Competencia oficial con tiempos certificados y ranking",
                TipoEvento.Campeonato => "Competencia con premios y posiciones oficiales",
                TipoEvento.Recreativo => "Evento recreativo sin carácter competitivo formal",
                TipoEvento.Entrenamiento => "Sesión de entrenamiento grupal guiada",
                TipoEvento.Clasificatorio => "Evento para clasificar a competencias mayores",
                _ => "Evento deportivo"
            };
        }

        public static bool RequiereInscripcion(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => true,
                TipoEvento.Campeonato => true,
                TipoEvento.Clasificatorio => true,
                _ => false // Recreativo y Entrenamiento pueden no requerir inscripción formal
            };
        }

        public static bool PermiteMultipleDistancias(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => true,
                TipoEvento.Campeonato => true,
                TipoEvento.Recreativo => true,
                _ => false
            };
        }

        public static bool RequiereJueces(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => true,
                TipoEvento.Campeonato => true,
                TipoEvento.Clasificatorio => true,
                _ => false
            };
        }

        public static bool EsCompetitivo(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.CarreraOficial => true,
                TipoEvento.Campeonato => true,
                TipoEvento.Clasificatorio => true,
                _ => false
            };
        }

        public static bool EsFormativo(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.Entrenamiento => true,
                TipoEvento.Recreativo => true,
                _ => false
            };
        }

        public static string GetNivelDificultad(this TipoEvento tipo)
        {
            return tipo switch
            {
                TipoEvento.Campeonato => "Alto",
                TipoEvento.Clasificatorio => "Alto",
                TipoEvento.CarreraOficial => "Medio",
                TipoEvento.Entrenamiento => "Bajo",
                TipoEvento.Recreativo => "Bajo",
                _ => "No especificado"
            };
        }
    }
}