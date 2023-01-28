using Microsoft.EntityFrameworkCore;
using SecureBank.DAL.DBModels;
using SecureBank.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL
{
    public class PortalDBContext : DbContext
    {
        public PortalDBContext(DbContextOptions<PortalDBContext> options) : base(options)
        {
        }

        public DbSet<UserDBModel> UserData { get; set; }
        public DbSet<SessionDBModel> Sessions { get; set; }
        public DbSet<TransactionDBModel> Transactions { get; set; }

        public DbSet<TransactionsByDayResp> TransactionsGroupedByDay { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TransactionsByDayResp>().HasNoKey();
        }
    }
}
