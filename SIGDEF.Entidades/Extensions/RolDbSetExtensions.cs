// Extensions/RolDbSetExtensions.cs
using Microsoft.EntityFrameworkCore;
using SIGDEF.Entidades;
using SIGDEF.Entidades.Enums;
using System.Collections.Generic;

namespace SIGDEF.Extensions
{
    public static class RolDbSetExtensions
    {
        /// <summary>
        /// Obtiene un rol por su tipo (enum)
        /// </summary>
        public static async Task<Rol?> GetByTipoAsync(this DbSet<Rol> roles, RolTipo tipo)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            string tipoString = tipo.ToString();
            return await roles
                .FirstOrDefaultAsync(r => r.Tipo == tipoString);
        }

        /// <summary>
        /// Obtiene un rol por su tipo (string)
        /// </summary>
        public static async Task<Rol?> GetByTipoAsync(this DbSet<Rol> roles, string tipo)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            if (string.IsNullOrWhiteSpace(tipo))
                throw new ArgumentException("El tipo no puede estar vacío", nameof(tipo));

            return await roles
                .FirstOrDefaultAsync(r => r.Tipo == tipo);
        }

        /// <summary>
        /// Obtiene el ID de un rol por su tipo (enum)
        /// </summary>
        public static async Task<int?> GetIdByTipoAsync(this DbSet<Rol> roles, RolTipo tipo)
        {
            var rol = await roles.GetByTipoAsync(tipo);
            return rol?.IdRol;
        }

        /// <summary>
        /// Verifica si existe un rol con el tipo especificado
        /// </summary>
        public static async Task<bool> ExistsByTipoAsync(this DbSet<Rol> roles, RolTipo tipo)
        {
            string tipoString = tipo.ToString();
            return await roles
                .AnyAsync(r => r.Tipo == tipoString);
        }

        /// <summary>
        /// Obtiene todos los roles ordenados por ID
        /// </summary>
        public static async Task<List<Rol>> GetAllOrderedAsync(this DbSet<Rol> roles)
        {
            return await roles
                .OrderBy(r => r.IdRol)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene roles por una lista de tipos
        /// </summary>
        public static async Task<List<Rol>> GetByTiposAsync(this DbSet<Rol> roles, params RolTipo[] tipos)
        {
            var tiposString = tipos.Select(t => t.ToString()).ToList();
            return await roles
                .Where(r => tiposString.Contains(r.Tipo))
                .ToListAsync();
        }

        /// <summary>
        /// Convierte un enum RolTipo al string que se almacena en la DB
        /// </summary>
        public static string ToDbString(this RolTipo tipo)
        {
            return tipo.ToString();
        }

        /// <summary>
        /// Intenta convertir un string de la DB a RolTipo
        /// </summary>
        public static bool TryParseFromDbString(string dbString, out RolTipo tipo)
        {
            return Enum.TryParse(dbString, out tipo);
        }
    }
}