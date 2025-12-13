// 📄 Services/MercadoPagoService.cs
// Usa estos using al principio:
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.Extensions.Options;
using Pagos.Models.Dtos;
using Pagos.Config;

namespace SIGDEF.Pagos;

public class MercadoPagoService
{
    private readonly string _accessToken;

    public MercadoPagoService(IOptions<MercadoPagoSettings> config)
    {
        _accessToken = config.Value.AccessToken;

        // Configurar SDK de MercadoPago
        MercadoPagoConfig.AccessToken = _accessToken;
    }

    public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
    {
        try
        {
            var client = new PaymentClient(); // ← Ahora funciona

            var paymentRequest = new PaymentCreateRequest
            {
                TransactionAmount = request.Amount,
                Description = request.Description,
                Payer = new PaymentPayerRequest
                {
                    Email = request.CustomerEmail,
                    FirstName = request.CustomerName
                }
            };

            Payment payment = await client.CreateAsync(paymentRequest);

            return new PaymentResponse
            {
                Success = payment.Status == "approved" || payment.Status == "pending",
                PaymentId = payment.Id.ToString(),
                Status = payment.Status ?? "unknown",
                PaymentUrl = GetPaymentUrl(payment)
            };
        }
        catch (Exception ex)
        {
            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private string GetPaymentUrl(Payment payment)
    {
        return payment.TransactionDetails?.ExternalResourceUrl
            ?? $"https://www.mercadopago.com.ar/checkout/v1/payment/{payment.Id}";
    }

    public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
    {
        try
        {
            var client = new PaymentClient();
            var payment = await client.GetAsync(long.Parse(paymentId));

            return new PaymentResponse
            {
                Success = true,
                PaymentId = payment.Id.ToString(),
                Status = payment.Status,
                PaymentUrl = GetPaymentUrl(payment)
            };
        }
        catch (Exception ex)
        {
            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

}