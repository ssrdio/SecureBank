using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.Transaction
{
    public class DepositRequest
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public double Amount { get; set; }
        public string Reason { get; set; }
    }
}
