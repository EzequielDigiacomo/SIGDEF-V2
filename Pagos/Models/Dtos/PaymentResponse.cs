using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pagos.Models.Dtos
{
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ErrorMessage { get; set; }
    }
}
