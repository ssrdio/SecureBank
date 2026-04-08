using SecureBank.DAL.DBModels;
using Microsoft.AspNetCore.Http;
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
        string GetIndexViewName(HttpContext httpContext = null);

        TransactionDBModel Details(int? id, HttpContext httpContext = null);

        bool Create(TransactionDBModel transactionTable, HttpContext httpContext = null);

        DataTableResp<TransactionResp> GetTransactions(string userName, string search, int start, int lenght, HttpContext httpContext = null);
        List<TransactionsByDayResp> GetTransactionsByDay(string userName, HttpContext httpContext = null);
    }
}
