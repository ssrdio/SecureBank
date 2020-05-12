using SecureBank.DAL.DAO;
using NLog;
using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Services
{
    public class TransactionBL : ITransactionBL
    {
        protected readonly ITransactionDAO _transactionDAO;

        public TransactionBL(ITransactionDAO transactionDAO)
        {
            _transactionDAO = transactionDAO;
        }

        public virtual bool Create(TransactionDBModel transaction)
        {
            return _transactionDAO.Add(transaction);
        }

        public virtual void Delete()
        {
            return;
        }

        public virtual void Edit()
        {
            return;
        }

        public virtual TransactionDBModel Details(int? id)
        {
            if(!id.HasValue)
            {
                return null;
            }

            TransactionDBModel transaction = _transactionDAO.Get(id.Value);

            return transaction;
        }

        public virtual DataTableResp<TransactionResp> GetTransactions(string userName, string search, int start, int lenght)
        {
            try
            {
                List<TransactionResp> transactions = _transactionDAO.GetTransactions(userName, search);

                return new DataTableResp<TransactionResp>(
                    recordsTotal: transactions.Count,
                    recordsFiltered: transactions.Count,
                    data: transactions.Skip(start).Take(lenght).ToList());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public virtual List<TransactionsByDayResp> GetTransactionsByDay(string userName)
        {
            return _transactionDAO.GetTransactionsByDay(userName);
        }

        public virtual string GetIndexViewName()
        {
            return "Index";
        }
    }
}
