using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGDEF.Entidades.Enums
{
    public enum TipoEvento
    {
        CarreraOficial = 1,      // Competencia oficial con ranking
        Campeonato = 2,              // Competencia con premios
        Recreativo = 3,          // Demostración sin competencia
        Entrenamiento = 4,       // Sesión de entrenamiento grupal
        Clasificatorio = 5,      // Para clasificar a otro evento
    }
}
