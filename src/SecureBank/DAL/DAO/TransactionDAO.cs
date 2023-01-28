using CommonUtils;
using Microsoft.EntityFrameworkCore;
using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using SecureBank.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL.DAO
{
    public class TransactionDAO : ITransactionDAO
    {
        protected readonly PortalDBContext _customerContext;

        public TransactionDAO(PortalDBContext customerContext)
        {
            _customerContext = customerContext;
        }

        public virtual TransactionDBModel Get(int id)
        {
            return _customerContext.Transactions
                .Where(x => x.Id == id)
                .SingleOrDefault();
        }

        public virtual double GetAccountBalance(string userName)
        {
            double income = _customerContext.Transactions.Where(t => t.ReceiverId == userName).Sum(t => t.Amount);
            double outcome = _customerContext.Transactions.Where(t => t.SenderId == userName).Sum(t => t.Amount);

            return income - outcome;
        }

        public virtual List<TransactionResp> GetTransactions()
        {
            return _customerContext.Transactions
                .OrderByDescending(t => t.TransactionDateTime)
                .Select(x => new TransactionResp
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    ReceiverId = x.ReceiverId,
                    DateTime = x.TransactionDateTime.ToShortDateString(),
                    Reason = x.Reason,
                    Amount = x.Amount,
                    Reference = x.Reference
                })
                .ToList();
        }

        public virtual RevenueAndExpenses GetRevenueAndExpenses(string myUsername)
        {
            double revenue = _customerContext.Transactions
                .Where(x => x.ReceiverId == myUsername)
                .Sum(x => x.Amount);

            double expenses = _customerContext.Transactions
                .Where(x => x.SenderId == myUsername)
                .Sum(x => x.Amount);

            return new RevenueAndExpenses { Revenue = revenue, Expenses = expenses };
        }

        public virtual List<TransactionsByDayResp> GetTransactionsByDay(string myUsername)
        {
            string withdrawalQuery = $@"
                    SELECT TransactionDateTime, SUM(amount) as Amount
                        FROM Transactions
                    WHERE senderId='{myUsername}'
                    GROUP BY TransactionDateTime";

            List<TransactionsByDayResp> withdrawals = _customerContext.TransactionsGroupedByDay
                .FromSqlRaw(withdrawalQuery)
                .ToList();

            string depositQuery = $@"
                    SELECT TransactionDateTime, SUM(amount) as Amount
                        FROM Transactions
                    WHERE receiverId='{myUsername}'
                    GROUP BY TransactionDateTime";

            List<TransactionsByDayResp> deposits = _customerContext.TransactionsGroupedByDay
                .FromSqlRaw(depositQuery)
                .ToList();

            List<TransactionsByDayResp> transactionsByDayRespList = new List<TransactionsByDayResp>();

            transactionsByDayRespList.AddRange(deposits);
            transactionsByDayRespList.AddRange(withdrawals
                .Select(x => new TransactionsByDayResp
                {
                    Amount = x.Amount * -1,
                    TransactionDateTime = x.TransactionDateTime
                }));

            return transactionsByDayRespList
                .GroupBy(x => new { x.TransactionDateTime.Year, x.TransactionDateTime.Month, x.TransactionDateTime.Day })
                .Select(x => new TransactionsByDayResp
                {
                    Amount = x.Sum(c => c.Amount),
                    TransactionDateTime = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day)
                })
                .OrderBy(x => x.TransactionDateTime)
                .ToList();
        }

        public List<TransactionsByDayResp> GetTransactionsByDayCtfCheck(string myUsername)
        {
            return _customerContext.Transactions
                .Where(x => x.SenderId == myUsername || x.ReceiverId == myUsername)
                .Select(x => new
                {
                    x.TransactionDateTime,
                    x.Amount,
                    x.ReceiverId
                })
                .ToList()
                .GroupBy(x => new { x.TransactionDateTime.Year, x.TransactionDateTime.Month, x.TransactionDateTime.Day, x.ReceiverId })
                .Select(x => new TransactionsByDayResp
                {
                    Amount = x.Key.ReceiverId == myUsername ? x.Sum(c => c.Amount) : x.Sum(c => c.Amount) * -1,
                    TransactionDateTime = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day)
                })
                .GroupBy(x => x.TransactionDateTime)
                .Select(x => new TransactionsByDayResp
                {
                    Amount = x.Sum(c => c.Amount),
                    TransactionDateTime = x.Key
                })
                .OrderBy(x => x.TransactionDateTime)
                .ToList();

        }

        public virtual bool MakeDeposit(DepositRequest request)
        {
            TransactionDBModel transaction = new TransactionDBModel
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Amount = request.Amount,
                Reason = request.Reason,
                TransactionDateTime = DateTime.UtcNow
            };

            _customerContext.Transactions.Add(transaction);

            int changes = _customerContext.SaveChanges();
            return changes > 0;
        }

        public virtual int MakeRandomTransactions(string toUserId)
        {
            Random rand = new Random();
            int randTrans = rand.Next(1, 15);

            List<UserDBModel> users = _customerContext.UserData.ToList();
            for (int i = 0; i < randTrans; i++)
            {
                TransactionDBModel transactionTable = new TransactionDBModel
                {
                    Amount = rand.NextDouble() * rand.Next(10, 1000),
                    TransactionDateTime = DateTime.UtcNow.AddSeconds(rand.Next(0, 4320000) * -1),
                    Reason = StringUtils.GetRandomFriendlyString(5),
                    ReceiverId = i % 2 == 0 ? toUserId : users[new Random().Next(0, users.Count)].UserName,
                    SenderId = i % 2 == 0 ? users[new Random().Next(0, users.Count)].UserName : toUserId
                };
                _customerContext.Transactions.Add(transactionTable);
            }

            return _customerContext.SaveChanges();
        }

        public virtual bool Pay(DepositRequest request)
        {
            TransactionDBModel transaction = new TransactionDBModel
            {
                SenderId = request.SenderId,
                ReceiverId = "store",
                Amount = request.Amount,
                Reason = request.Reason,
                TransactionDateTime = DateTime.UtcNow
            };

            _customerContext.Transactions.Add(transaction);

            int changes = _customerContext.SaveChanges();
            return changes > 0;
        }

        public virtual List<TransactionResp> GetTransactions(string userName, string search)
        {
            string query = $@"
                SELECT *
                    FROM Transactions
                WHERE (ReceiverId='{userName}' OR SenderId='{userName}')
                    AND Reason LIKE '%{search}%'";

            return _customerContext.Transactions
                .FromSqlRaw(query)
                .OrderByDescending(x => x.TransactionDateTime)
                .Select(x => new TransactionResp
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    ReceiverId = x.ReceiverId,
                    DateTime = x.TransactionDateTime.ToShortDateString(),
                    Reason = x.Reason,
                    Amount = x.Amount,
                    Reference = x.Reference
                })
                .ToList();
        }

        public virtual List<TransactionResp> GetTransactionsCtfCheck(string userName, string search)
        {
            if (search == null)
            {
                search = string.Empty;
            }

            return _customerContext.Transactions
                .Where(x => x.ReceiverId == userName || x.SenderId == userName)
                .Where(x => x.Reason.Contains(search))
                .OrderByDescending(x => x.TransactionDateTime)
                .Select(x => new TransactionResp
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    ReceiverId = x.ReceiverId,
                    DateTime = x.TransactionDateTime.ToShortDateString(),
                    Reason = x.Reason,
                    Amount = x.Amount,
                    Reference = x.Reference
                })
                .ToList();
        }

        public bool Add(TransactionDBModel transaction)
        {
            _customerContext.Transactions.Add(transaction);

            int changes = _customerContext.SaveChanges();
            return changes > 0;
        }
    }
}