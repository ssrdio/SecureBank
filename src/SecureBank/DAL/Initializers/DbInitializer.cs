using SecureBank.Models.User;
using CommonUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL.Initializers
{
    public class DbInitializer : IDbInitializer
    {
        private readonly PortalDBContext _context;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public DbInitializer(PortalDBContext context)
        {
            _context = context;
        }

        public void Initialize(IApplicationBuilder app, string admin, string adminPassword, string userPassword)
        {
            //context.Database.EnsureDeleted();
            _logger.Info("Creating database");
            _context.Database.EnsureCreated();
            _logger.Info("Database created");

            List<string> userNames = AddUsers(admin, adminPassword, userPassword);
            if (userNames == null)
            {
                return;
            }

            AddTransactions(userNames);
        }

        private List<string> AddUsers(string admin, string adminPassword, string userPassword)
        {
            if (_context.UserData.Any())
            {
                return null; // DB seeded
            }

            List<UserDBModel> sampleUsers = new List<UserDBModel>
            {
                new UserDBModel{ Name="Carson", Surname="Alexander", UserName="carson.alexander@ssrd.io"},
                new UserDBModel{ Name="Merdith", Surname="Alonso", UserName="merdith.alonso@ssrd.io"},
                new UserDBModel{ Name="Arturo", Surname="Anand", UserName="arturo.anad@ssrd.io"},
                new UserDBModel{ Name="Gytis", Surname="Barzdukas", UserName="gytis.barzdukas@ssrd.io"},
                new UserDBModel{ Name="Yan", Surname="Li", UserName="yan.li@ssrd.io"},
                new UserDBModel{ Name="Peggy", Surname="Justice", UserName="peggy.justice@ssrd.io"},
                new UserDBModel{ Name="Laura", Surname="Norman", UserName="laura.norman@ssrd.io"},
                new UserDBModel{ Name="Nino", Surname="Olivetto", UserName="nino.olivetto@ssrd.io"},
                new UserDBModel{ Name="Tester", Surname="Test", UserName="tester@ssrd.io"}
            };

            foreach (UserDBModel user in sampleUsers)
            {
                user.Password = userPassword;
                user.Confirmed = true;

                _context.UserData.Add(user);
            }

            _context.SaveChanges();

            if (!string.IsNullOrEmpty(admin) && !string.IsNullOrEmpty(adminPassword))
            {
                UserDBModel adminDbModel = new UserDBModel { Name = "admin", Surname = "admin", UserName = admin, Password = adminPassword, Confirmed = true, Role = 100 };
                _context.UserData.Add(adminDbModel);

                _context.SaveChanges();

                sampleUsers.Add(adminDbModel);
            }

            return sampleUsers
                .Select(x => x.UserName)
                .ToList();
        }

        private void AddTransactions(List<string> userNames, int numberOfTransactions = 30)
        {
            if (_context.Transactions.Any())
            {
                return;   // DB has been seeded
            }

            Random random = new Random();

            for (int i = 0; i < numberOfTransactions; i++)
            {
                TransactionDBModel transactionTable = new TransactionDBModel
                {
                    Amount = random.NextDouble() * random.Next(10, 1000),
                    TransactionDateTime = DateTime.UtcNow.AddSeconds(random.Next(0, 4320000) * -1),
                    Reason = StringUtils.GetRandomFriendlyString(5),
                    ReceiverId = userNames[random.Next(0, userNames.Count)],
                    SenderId = userNames[random.Next(0, userNames.Count)]
                };

                _context.Transactions.Add(transactionTable);
            }

            _context.SaveChanges();
        }

        public void Create(IApplicationBuilder app)
        {
            _logger.Info("Creating database");
            _context.Database.EnsureCreated();
            _logger.Info("Database created");
        }
    }
}
