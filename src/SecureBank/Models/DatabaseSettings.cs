using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models
{
    public class DatabaseSettings
    {
        public string UserId { get; set; }
        public string UserPass { get; set; }
        public string Server { get; set; }
        public string ServerPort { get; set; }
        public string Database { get; set; }
    }
}
