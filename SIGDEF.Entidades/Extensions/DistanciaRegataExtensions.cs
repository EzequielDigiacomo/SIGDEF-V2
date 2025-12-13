using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGDEF.Entidades.Enums;
namespace SIGDEF.Entidades.Extensions
{
    public static class DistanciaRegataExtensions
    {
        /// <summary>
        /// Devuelve el código corto para mostrar (200m, 5K, 10K, etc.)
        /// </summary>
        public static string ToDisplayString(this DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => "200m",
                DistanciaRegata.TrecientosCincuentaMetros => "350m",
                DistanciaRegata.QuatroCientosMetros => "400m",
                DistanciaRegata.QuinientosMetros => "500m",
                DistanciaRegata.MilMetros => "1000m",
                DistanciaRegata.DosKilometros => "2K",
                DistanciaRegata.TresKilometros => "3K",
                DistanciaRegata.CincoKilometros => "5K",
                DistanciaRegata.DiezKilometros => "10K",
                DistanciaRegata.QuinceKilometros => "15K",
                DistanciaRegata.VeintiDosKilometros => "22K",
                DistanciaRegata.VeintiCincoKilometros => "25K",
                DistanciaRegata.TreintaDosKilometros => "32K",
                _ => distancia.ToString()
            };
        }

        /// <summary>
        /// Devuelve el nombre completo para mostrar
        /// </summary>
        public static string ToNombreCompleto(this DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => "Doscientos Metros (200m)",
                DistanciaRegata.TrecientosCincuentaMetros => "Trescientos Cincuenta Metros (350m)",
                DistanciaRegata.QuatroCientosMetros => "Cuatrocientos Metros (400m)",
                DistanciaRegata.QuinientosMetros => "Quinientos Metros (500m)",
                DistanciaRegata.MilMetros => "Mil Metros (1000m)",
                DistanciaRegata.DosKilometros => "Dos Kilómetros (2K)",
                DistanciaRegata.TresKilometros => "Tres Kilómetros (3K)",
                DistanciaRegata.CincoKilometros => "Cinco Kilómetros (5K)",
                DistanciaRegata.DiezKilometros => "Diez Kilómetros (10K)",
                DistanciaRegata.QuinceKilometros => "Quince Kilómetros (15K)",
                DistanciaRegata.VeintiDosKilometros => "Veintidós Kilómetros (22K)",
                DistanciaRegata.VeintiCincoKilometros => "Veinticinco Kilómetros (25K)",
                DistanciaRegata.TreintaDosKilometros => "Treinta y Dos Kilómetros (32K)",
                _ => distancia.ToString()
            };
        }

        /// <summary>
        /// Devuelve la distancia en metros
        /// </summary>
        public static decimal GetMetros(this DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => 200,
                DistanciaRegata.TrecientosCincuentaMetros => 350,
                DistanciaRegata.QuatroCientosMetros => 400,
                DistanciaRegata.QuinientosMetros => 500,
                DistanciaRegata.MilMetros => 1000,
                DistanciaRegata.DosKilometros => 2000,
                DistanciaRegata.TresKilometros => 3000,
                DistanciaRegata.CincoKilometros => 5000,
                DistanciaRegata.DiezKilometros => 10000,
                DistanciaRegata.QuinceKilometros => 15000,
                DistanciaRegata.VeintiDosKilometros => 22000,
                DistanciaRegata.VeintiCincoKilometros => 25000,
                DistanciaRegata.TreintaDosKilometros => 32000,
                _ => 0
            };
        }

        /// <summary>
        /// Devuelve la distancia en kilómetros
        /// </summary>
        public static decimal GetKilometros(this DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => 0.2m,
                DistanciaRegata.TrecientosCincuentaMetros => 0.35m,
                DistanciaRegata.QuatroCientosMetros => 0.4m,
                DistanciaRegata.QuinientosMetros => 0.5m,
                DistanciaRegata.MilMetros => 1.0m,
                DistanciaRegata.DosKilometros => 2.0m,
                DistanciaRegata.TresKilometros => 3.0m,
                DistanciaRegata.CincoKilometros => 5.0m,
                DistanciaRegata.DiezKilometros => 10.0m,
                DistanciaRegata.QuinceKilometros => 15.0m,
                DistanciaRegata.VeintiDosKilometros => 22.0m,
                DistanciaRegata.VeintiCincoKilometros => 25.0m,
                DistanciaRegata.TreintaDosKilometros => 32.0m,
                _ => 0
            };
        }

        /// <summary>
        /// Devuelve la unidad de medida principal (metros o kilómetros)
        /// </summary>
        public static string GetUnidad(this DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros or
                DistanciaRegata.TrecientosCincuentaMetros or
                DistanciaRegata.QuatroCientosMetros or
                DistanciaRegata.QuinientosMetros or
                DistanciaRegata.MilMetros => "metros",

                _ => "kilómetros"
            };
        }

        /// <summary>
        /// Determina si es una distancia de pista (menor o igual a 1500m)
        /// </summary>
        public static bool EsDistanciaPista(this DistanciaRegata distancia)
        {
            return distancia.GetMetros() <= 1500;
        }

        /// <summary>
        /// Determina si es una distancia de ruta/carretera
        /// </summary>
        public static bool EsDistanciaRuta(this DistanciaRegata distancia)
        {
            return distancia.GetMetros() >= 5000;
        }

        /// <summary>
        /// Devuelve el tipo de carrera según la distancia
        /// </summary>
        public static string GetTipoCarrera(this DistanciaRegata distancia)
        {
            var metros = distancia.GetMetros();

            return metros switch
            {
                <= 1000 => "Sprint",
                <= 5000 => "Medio Fonto",
                <= 10000 => "Maraton Corto",
                <= 21000 => "Media Maratón",
                <= 32000 => "Maratón",
                > 42200 => "Ultra Distancia"
            };
        }

        /// <summary>
        /// Devuelve todas las distancias de un tipo específico
        /// </summary>
        public static List<DistanciaRegata> GetPorTipo(string tipo)
        {
            return Enum.GetValues<DistanciaRegata>()
                .Where(d => d.GetTipoCarrera() == tipo)
                .ToList();
        }
    }
}

