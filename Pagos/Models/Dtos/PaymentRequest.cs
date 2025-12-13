using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pagos.Models.Dtos
{
    public class PaymentRequest
    {
        public string Gateway { get; set; } = "MercadoPago";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "ARS";
        public string Description { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
