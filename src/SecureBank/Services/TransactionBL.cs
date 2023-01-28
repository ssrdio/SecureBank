using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using System.Collections.Generic;
using System.Linq;

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
            bool result = CheckTransaction(transaction);
            if(!result)
            {
                return false;
            }

            return _transactionDAO.Add(transaction);
        }

        protected virtual bool CheckTransaction(TransactionDBModel transaction)
        {
            if(transaction.Amount < 0)
            {
                return false;
            }

            if(string.IsNullOrEmpty(transaction.SenderId) || string.IsNullOrEmpty(transaction.ReceiverId))
            {
                return false;
            }

            double accountBalance = _transactionDAO.GetAccountBalance(transaction.SenderId);
            if (accountBalance < 0)
            {
                return false;
            }

            if (accountBalance < transaction.Amount)
            {
                return false;
            }

            return true;
        }

        public virtual TransactionDBModel Details(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            TransactionDBModel transaction = _transactionDAO.Get(id.Value);

            return transaction;
        }

        public virtual DataTableResp<TransactionResp> GetTransactions(string userName, string search, int start, int lenght)
        {
            List<TransactionResp> transactions = _transactionDAO.GetTransactions(userName, search);

            return new DataTableResp<TransactionResp>(
                recordsTotal: transactions.Count,
                recordsFiltered: transactions.Count,
                data: transactions.Skip(start).Take(lenght).ToList());
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
