// Helpers/RolValidationHelper.cs
using SIGDEF.Entidades.Enums;

namespace SIGDEF.Helpers
{
    public static class RolValidationHelper
    {
        private static readonly HashSet<int> _validRolIds = new() { 1, 2, 3, 4, 5, 6, 7 };

        /// <summary>
        /// Valida si un ID de rol es válido
        /// </summary>
        public static bool IsValidRolId(int rolId)
        {
            return _validRolIds.Contains(rolId);
        }

        /// <summary>
        /// Valida si un string puede ser convertido a RolTipo
        /// </summary>
        public static bool IsValidRolTipo(string tipo)
        {
            return Enum.TryParse<RolTipo>(tipo, out _);
        }

        /// <summary>
        /// Valida si un enum RolTipo es válido
        /// </summary>
        public static bool IsValidRolTipo(RolTipo tipo)
        {
            return Enum.IsDefined(typeof(RolTipo), tipo);
        }

        /// <summary>
        /// Obtiene todos los valores válidos del enum
        /// </summary>
        public static List<KeyValuePair<int, string>> GetValidRoles()
        {
            return Enum.GetValues(typeof(RolTipo))
                .Cast<RolTipo>()
                .Select(t => new KeyValuePair<int, string>((int)t, t.ToString()))
                .ToList();
        }
    }
}