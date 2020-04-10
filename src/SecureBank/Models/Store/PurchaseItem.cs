using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.Store
{
    public class PurcahseHistoryItemResp
    {
        public DateTime PurchaseTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string UserName { get; set; }
    }
}
