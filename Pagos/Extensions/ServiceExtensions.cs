using Microsoft.Extensions.Configuration;
using SIGDEF.Pagos;
using Pagos.Config;
using SIGDEF.Pagos.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar MercadoPago
        services.Configure<MercadoPagoSettings>(configuration.GetSection("MercadoPago"));

        // Registrar servicios
        services.AddScoped<MercadoPagoService>();
        services.AddScoped<PaymentService>();

        // Configurar HttpClient para MercadoPago si necesitas llamadas directas
        services.AddHttpClient("MercadoPago", client =>
        {
            client.BaseAddress = new Uri("https://api.mercadopago.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
