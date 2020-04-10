using Microsoft.EntityFrameworkCore;
using StoreAPI.DAL.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.DAL
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        { }

        public DbSet<StoreItemTable> StoreItems { get; set; }
        public DbSet<PurchaseItemTable> Purchases { get; set; }
    }
}
