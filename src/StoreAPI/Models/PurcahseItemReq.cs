using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.Models
{
    public class PurcahseItemReq
    {
        public int StoreItemId { get; set; }
        public string Username { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
