    using Microsoft.Extensions.Logging;
    using Pagos.Models.Dtos;
 

    namespace SIGDEF.Pagos.Services;

    public class PaymentService
    {
        private readonly MercadoPagoService _mercadoPagoService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            MercadoPagoService mercadoPagoService,
            ILogger<PaymentService> logger)
        {
            _mercadoPagoService = mercadoPagoService;
            _logger = logger;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            _logger.LogInformation("Procesando pago con gateway: {Gateway}", request.Gateway);

            return request.Gateway.ToLower() switch
            {
                "mercadopago" or "mp" => await _mercadoPagoService.CreatePaymentAsync(request),
                _ => throw new NotImplementedException($"Gateway no soportado: {request.Gateway}")
            };
        }

        public async Task<PaymentResponse> GetPaymentStatusAsync(string gateway, string paymentId)
        {
            return gateway.ToLower() switch
            {
                "mercadopago" or "mp" => await _mercadoPagoService.GetPaymentStatusAsync(paymentId),
                _ => throw new NotImplementedException($"Gateway no soportado: {gateway}")
            };
        }

        // Método para validar si un gateway está disponible
        public bool IsGatewayAvailable(string gateway)
        {
            var availableGateways = new[] { "mercadopago", "mp" };
            return availableGateways.Contains(gateway.ToLower());
        }
    }
