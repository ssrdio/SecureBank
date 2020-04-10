using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Models.Store
{
    public class BuyProductReq
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
