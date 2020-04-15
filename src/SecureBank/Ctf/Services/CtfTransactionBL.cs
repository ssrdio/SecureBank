using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SecureBank.Ctf;
using SecureBank.Ctf.Models;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using SecureBank.Services;

namespace SecureBank.Ctf.Services
{
    public class CtfTransactionBL : TransactionBL
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly CtfOptions _ctfOptions;

        public CtfTransactionBL(ITransactionDAO transactionDAO, IHttpContextAccessor httpContextAccessor, IOptions<CtfOptions> ctfOptions) : base(transactionDAO)
        {
            _httpContextAccessor = httpContextAccessor;

            _ctfOptions = ctfOptions.Value;
        }

        public override bool Create(TransactionDBModel transactionTable)
        {
            string userName = _httpContextAccessor.HttpContext.GetUserName();

            if (transactionTable.SenderId != userName)
            {
                CtfChallangeModel invalidModelChallenge = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.InvalidModel)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Cookies.Append(invalidModelChallenge.FlagKey, invalidModelChallenge.Flag);
            }

            return base.Create(transactionTable);
        }

        public override TransactionDBModel Details(int? id)
        {
            TransactionDBModel transaction = base.Details(id);
            if (transaction == null)
            {
                return null;
            }

            string userName = _httpContextAccessor.HttpContext.GetUserName();
            string role = _httpContextAccessor.HttpContext.GetRole();

            if (role != "admin" && transaction.SenderId != userName && transaction.ReceiverId != userName)
            {
                CtfChallangeModel enumerationChallange = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.Enumeration)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Headers.Add(enumerationChallange.FlagKey, enumerationChallange.Flag);
            }

            return transaction;
        }

        public override void Edit()
        {
            CtfChallangeModel unkonwChallange = _ctfOptions.CtfChallanges
                .Where(x => x.Type == CtfChallengeTypes.UnknownGeneration)
                .Single();

            _httpContextAccessor.HttpContext.Response.Cookies.Append(unkonwChallange.FlagKey, unkonwChallange.Flag);

            base.Edit();
        }

        public override void Delete()
        {
            CtfChallangeModel unkonwChallange = _ctfOptions.CtfChallanges
                .Where(x => x.Type == CtfChallengeTypes.UnknownGeneration)
                .Single();

            _httpContextAccessor.HttpContext.Response.Cookies.Append(unkonwChallange.FlagKey, unkonwChallange.Flag);

            base.Delete();
        }

        public override DataTableResp<TransactionResp> GetTransactions(string userName, string search, int start, int lenght)
        {
            CtfChallangeModel sqlInjectionChallange = _ctfOptions.CtfChallanges
                .Where(x => x.Type == CtfChallengeTypes.SqlInjection)
                .Single();

            DataTableResp<TransactionResp> paginatedTransactions = base.GetTransactions(userName, search, start, lenght);
            if(paginatedTransactions == null)
            {
                paginatedTransactions = new DataTableResp<TransactionResp>();
            }

            string validSearch = search;
            if(search == null || search.All(x => "%".Contains(x)))
            {
                validSearch = null;
            }

            List<TransactionResp> validTransactions = _transactionDAO.GetTransactionsCtfCheck(userName, validSearch);

            if (validTransactions.Count != paginatedTransactions.RecordsTotal)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add(sqlInjectionChallange.FlagKey, sqlInjectionChallange.Flag);
            }
            else
            {
                foreach (TransactionResp transaction in paginatedTransactions.Data)
                {
                    if (!validTransactions.Any(x => x.IsEqual(transaction)))
                    {
                        _httpContextAccessor.HttpContext.Response.Headers.Add(sqlInjectionChallange.FlagKey, sqlInjectionChallange.Flag);
                        break;
                    }
                }
            }

            bool xss = paginatedTransactions.Data.Any(x => CtfConstants.XXS_KEYVORDS.Any(c =>
                (x.SenderId?.Contains(c) ?? false) || (x.ReceiverId?.Contains(c) ?? false) || (x.Reason?.Contains(c) ?? false) || (x.Reference?.Contains(c) ?? false)));
            if (xss)
            {
                CtfChallangeModel xxsChallange = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.Xss)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Headers.Add(xxsChallange.FlagKey, xxsChallange.Flag);
            }

            return base.GetTransactions(userName, search, start, lenght);
        }

        public override List<TransactionsByDayResp> GetTransactionsByDay(string userName)
        {
            CtfChallangeModel sqlInjectionChallange = _ctfOptions.CtfChallanges
                .Where(x => x.Type == CtfChallengeTypes.SqlInjection)
                .Single();

            List<TransactionsByDayResp> transactions;

            try
            {
                transactions = _transactionDAO.GetTransactionsByDay(userName);
            }
            catch (Exception)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add(sqlInjectionChallange.FlagKey, sqlInjectionChallange.Flag);
                return null;
            }

            List<TransactionsByDayResp> validTransactions = _transactionDAO.GetTransactionsByDayCtfCheck(userName);

            if (validTransactions.Count != transactions.Count)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add(sqlInjectionChallange.FlagKey, sqlInjectionChallange.Flag);
            }
            else
            {
                foreach (var transaction in transactions)
                {
                    if (!validTransactions.Any(x => x.IsEqual(transaction)))
                    {
                        _httpContextAccessor.HttpContext.Response.Headers.Add(sqlInjectionChallange.FlagKey, sqlInjectionChallange.Flag);
                        break;
                    }
                }
            }

            return transactions;
        }
    }
}
