using System.ComponentModel.DataAnnotations;

namespace SIGDEF.Entidades.DTOs.Federacion
{
    public class FederacionCreateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Cuit { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BancoNombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TipoCuenta { get; set; } = string.Empty;

        [MaxLength(50)]
        public string NumeroCuenta { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TitularCuenta { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmailCobro { get; set; } = string.Empty;
    }
}