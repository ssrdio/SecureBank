using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models
{
    public class AppSettings
    {
        public AppSettings() 
        {
            StoreEndpoint = new ApiEndpoint();
            SmtpCredentials = new SmtpCredentials();
            Ctf = new CtfConfig();
        }
        public string BaseUrl { get; set; }

        public ApiEndpoint StoreEndpoint { get; set; }
        public SmtpCredentials SmtpCredentials { get; set; }
        public CtfConfig Ctf { get; set; }

        public bool IgnoreEmails { get; set; }
    }
}
