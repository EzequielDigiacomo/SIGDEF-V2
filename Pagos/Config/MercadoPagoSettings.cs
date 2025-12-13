using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pagos.Config
{
    public class MercadoPagoSettings
    {
        public string AccessToken { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public bool SandboxMode { get; set; } = true;
    }
}
