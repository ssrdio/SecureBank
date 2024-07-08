using CommonUtils;
using Microsoft.AspNetCore.Builder;
using NLog;
using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using SecureBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void Initialize(string admin, string adminPassword)
        {
            AddAdmin(admin, adminPassword);
            AddCredit();
        }

        public void Seed(string userPassword)
        {
            List<string> userNames = AddUsers(userPassword);
            if (userNames == null)
            {
                return;
            }

            AddTransactions(userNames);
        }

        private void AddAdmin(string admin, string adminPassword)
        {
            if(string.IsNullOrEmpty(admin) || string.IsNullOrEmpty(adminPassword))
            {
                return;
            }

            bool exists = _context.UserData
                .Where(x => x.UserName == admin)
                .Any();
            if(exists)
            {
                return;
            }

            UserDBModel adminDbModel = new UserDBModel 
            {
                Name = "admin",
                Surname = "admin",
                UserName = admin,
                Password = adminPassword,
                Confirmed = true,
                Role = 100 
            };
            _context.UserData.Add(adminDbModel);

            int changes = _context.SaveChanges();
            if(changes <= 0)
            {
                throw new Exception("Failed to add admin");
            }

            GiveMoney(new List<string> { adminDbModel.UserName });
        }

        private void AddCredit()
        {
            bool exists = _context.UserData
                .Where(x => x.UserName == SecureBankConstants.CREDIT_USERNAME)
                .Any();
            if (exists)
            {
                return;
            }

            UserDBModel creditDbModel = new UserDBModel 
            { 
                Name = "credit",
                Surname = "credit",
                UserName = SecureBankConstants.CREDIT_USERNAME,
                Password = Guid.NewGuid().ToString(),
                Confirmed = true,
                Role = 100
            };
            _context.UserData.Add(creditDbModel);

            int changes = _context.SaveChanges();
            if (changes <= 0)
            {
                throw new Exception("Failed to add credit");
            }
        }

        private List<string> AddUsers(string userPassword)
        {
            if (_context.UserData.Any())
            {
                _logger.Info($"Skip seeding users");
                return null; // DB seeded
            }

            _logger.Info($"Seeding users");

            List<UserDBModel> sampleUsers = new List<UserDBModel>
            {
                new UserDBModel{ Name="Super", Surname="Developer", UserName="developer@ssrd.io"},
                new UserDBModel{ Name="Master", Surname="Yoda", UserName="yoda@ssrd.io"},
                new UserDBModel{ Name="Carson", Surname="Alexander", UserName="carson.alexander@ssrd.io"},
                new UserDBModel{ Name="Merdith", Surname="Alonso", UserName="merdith.alonso@ssrd.io"},
                new UserDBModel{ Name="Arturo", Surname="Anand", UserName="arturo.anad@ssrd.io"},
                new UserDBModel{ Name="Gytis", Surname="Barzdukas", UserName="gytis.barzdukas@ssrd.io"},
                new UserDBModel{ Name="Yan", Surname="Li", UserName="yan.li@ssrd.io"},
                new UserDBModel{ Name="Peggy", Surname="Justice", UserName="peggy.justice@ssrd.io"},
                new UserDBModel{ Name="Laura", Surname="Norman", UserName="laura.norman@ssrd.io"},
                new UserDBModel{ Name="Nino", Surname="Olivetto", UserName="nino.olivetto@ssrd.io"},
                new UserDBModel{ Name="Electricity", Surname="Non Stop d.o.o.", UserName="electricity@ssrd.io"},
                new UserDBModel{ Name="Water", Surname="d.o.o.", UserName="water@ssrd.io"},
                new UserDBModel{ Name="Telephone", Surname="bird d.o.o.", UserName="internet@ssrd.io"},
                new UserDBModel{ Name="Gas", Surname="imporeted only d.o.o.", UserName="gas@ssrd.io"},
                new UserDBModel{ Name="groceries", Surname="mix", UserName="groceries@ssrd.io"},
                new UserDBModel{ Name="Janez", Surname="Novak", UserName="janeznovak@ssrd.io"},
                new UserDBModel{ Name="Tester", Surname="Test", UserName="tester@ssrd.io"},
                new UserDBModel {Name="Credit", Surname="Credit", UserName=SecureBankConstants.CREDIT_USERNAME,}
            };

            foreach (UserDBModel user in sampleUsers)
            {
                user.Password = userPassword;
                user.Confirmed = true;

                _context.UserData.Add(user);
            }

            _context.SaveChanges();

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

            for(int i = 0; i < 6; i++)
            {
                var electricity = userNames.FirstOrDefault(t => t.Contains("electricity"));
                var water = userNames.FirstOrDefault(t => t.Contains("water"));
                var internet = userNames.FirstOrDefault(t => t.Contains("internet"));
                var gas = userNames.FirstOrDefault(t => t.Contains("gas"));
                var groceries = userNames.FirstOrDefault(t => t.Contains("groceries"));

                _context.Transactions.Add(new TransactionDBModel 
                {
                    SenderId = electricity,
                });
            }

            _context.SaveChanges();
        }

        public void GiveMoneyToAll(double amount = 10000)
        {
            List<string> userNames = _context.UserData
                .Select(x => x.UserName)
                .ToList();

            GiveMoney(userNames, amount);
        }

        public void GiveMoney(List<string> userNames, double amount = 10000)
        {
            List<TransactionDBModel> transactions = new List<TransactionDBModel>();

            foreach(var user in userNames)
            {
                TransactionDBModel transactionDBModel = new TransactionDBModel
                {
                    Amount = amount,
                    TransactionDateTime = DateTime.UtcNow,
                    Reason = "top up",
                    ReceiverId = user,
                    SenderId = "SecureBank"
                };

                transactions.Add(transactionDBModel);
            }

            _context.Transactions.AddRange(transactions);

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
