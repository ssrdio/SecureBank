using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.Models
{
    public class PurcahseHistoryItemResp
    {
        public DateTime PurchaseTime { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        // only in admin response
        public string UserName { get; set; }
    }
}
