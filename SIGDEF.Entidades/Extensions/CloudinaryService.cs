using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using SIGDEF.Entidades.Extensions;
using System.Security.Principal;
namespace SIGDEF.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            try
            {
                // Validar que las configuraciones existan
                if (string.IsNullOrEmpty(config.Value.CloudName) ||
                    string.IsNullOrEmpty(config.Value.ApiKey) ||
                    string.IsNullOrEmpty(config.Value.ApiSecret))
                {
                    Console.WriteLine("⚠️ ADVERTENCIA: Configuración de Cloudinary incompleta");
                    _cloudinary = null;
                    return;
                }
                var account = new Account(
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );
                _cloudinary = new Cloudinary(account);

                Console.WriteLine("✅ Cloudinary inicializado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al inicializar Cloudinary: {ex.Message}");
                _cloudinary = null;
            }
        }
        public Cloudinary GetCloudinary()
        {
            if (_cloudinary == null)
            {
                throw new InvalidOperationException("Cloudinary no está configurado correctamente");
            }
            return _cloudinary;
        }
    }
}
    