using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.Transaction
{
    public class TransactionsByDayResp
    {
        public double Amount { get; set; }
        public DateTime TransactionDateTime { get; set; }
    }

    public static class TransactionsByDayRespExtensions
    {
        public static bool IsEqual(this TransactionsByDayResp thisResp, TransactionsByDayResp resp)
        {
            if (thisResp.Amount != resp.Amount)
            {
                return false;
            }

            if (thisResp.TransactionDateTime != resp.TransactionDateTime)
            {
                return false;
            }

            return true;
        }
    }
}
