using SecureBank.DAL.DBModels;
using SecureBank.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface ITransactionDAO
    {
        TransactionDBModel Get(int id);
        double GetAccountBalance(string userName);
        List<TransactionResp> GetTransactions();
        RevenueAndExpenses GetRevenueAndExpenses(string myUsername);
        List<TransactionsByDayResp> GetTransactionsByDay(string myUsername);
        List<TransactionsByDayResp> GetTransactionsByDayCtfCheck(string myUsername);
        bool MakeDeposit(DepositRequest request);
        int MakeRandomTransactions(string toUserId);
        bool Pay(DepositRequest request);
        List<TransactionResp> GetTransactions(string userName, string search);
        List<TransactionResp> GetTransactionsCtfCheck(string userName, string search);

        bool Add(TransactionDBModel transaction);
    }
}
