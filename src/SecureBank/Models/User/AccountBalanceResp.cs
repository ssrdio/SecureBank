using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.User
{
    public class AccountBalanceResp
    {
        public double Balance { get; set; }

        public AccountBalanceResp(double balance)
        {
            Balance = balance;
        }
    }
}
