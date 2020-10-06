using SecureBank.DAL.DBModels;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface ITransactionBL
    {
        string GetIndexViewName();

        TransactionDBModel Details(int? id);

        bool Create(TransactionDBModel transactionTable);

        DataTableResp<TransactionResp> GetTransactions(string userName, string search, int start, int lenght);
        List<TransactionsByDayResp> GetTransactionsByDay(string userName);
    }
}
