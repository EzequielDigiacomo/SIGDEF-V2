using Microsoft.Extensions.DependencyInjection;
using SIGDEF.Pagos;
using SIGDEF.Pagos.Services;

namespace Pagos.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMercadoPagoServices(this IServiceCollection services)
        {
            services.AddScoped<MercadoPagoService>();
            services.AddScoped<PaymentService>();
            return services;
        }
    }
}

