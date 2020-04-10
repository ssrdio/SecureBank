using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.Transaction
{
    public class TransactionResp
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string DateTime { get; set; }
        public string Reason { get; set; }
        public double Amount { get; set; }
        public string Reference { get; set; }

        public string SenderName { get; set; }
        public string SenderSurname { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverSurname { get; set; }
    }

    public static class TransactionRespExtensions
    {
        public static bool IsEqual(this TransactionResp thisResp, TransactionResp resp)
        {
            if (thisResp.Id != resp.Id)
            {
                return false;
            }

            if (thisResp.SenderId != resp.SenderId)
            {
                return false;
            }

            if (thisResp.ReceiverId != resp.ReceiverId)
            {
                return false;
            }

            if (thisResp.DateTime != resp.DateTime)
            {
                return false;
            }

            if (thisResp.Reason != resp.Reason)
            {
                return false;
            }

            if (thisResp.Amount != resp.Amount)
            {
                return false;
            }

            if (thisResp.Reference != resp.Reference)
            {
                return false;
            }

            if (thisResp.SenderName != resp.SenderName)
            {
                return false;
            }

            if (thisResp.SenderSurname != resp.SenderSurname)
            {
                return false;
            }

            if (thisResp.ReceiverName != resp.ReceiverName)
            {
                return false;
            }

            if (thisResp.ReceiverSurname != resp.ReceiverSurname)
            {
                return false;
            }

            return true;
        }
    }
}
