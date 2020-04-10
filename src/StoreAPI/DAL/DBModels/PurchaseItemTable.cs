using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.DAL.DBModels
{
    public class PurchaseItemTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Installments { get; set; }
        public int StoreItemId { get; set; }
        public string Username { get; set; }
        public int Quantity { get; set; }
        public DateTime DateTimePurchased { get; set; }
    }
}
