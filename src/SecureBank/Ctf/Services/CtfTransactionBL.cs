using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf;
using SecureBank.Ctf.Models;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using SecureBank.Services;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Authorization;

namespace SecureBank.Ctf.Services
{
    public class CtfTransactionBL : TransactionBL
    {
        private readonly IUserDAO _userDAO;

        private readonly CtfOptions _ctfOptions;

        public CtfTransactionBL(
            ITransactionDAO transactionDAO,
            IUserDAO userDAO,
            IOptions<CtfOptions> ctfOptions) : base(transactionDAO)
        {
            _userDAO = userDAO;

            _ctfOptions = ctfOptions.Value;
        }

        public override bool Create(TransactionDBModel transactionTable, HttpContext httpContext)
        {
            string userName = httpContext.GetUserName();

            if (transactionTable.SenderId != userName)
            {
                if (!_ctfOptions.CtfChallengeOptions.InvalidModelTransaction)
                {
                    return false;
                }

                CtfChallengeModel invalidModelChallenge = _ctfOptions.CtfChallenges
                    .Where(x => x.Type == CtfChallengeTypes.InvalidModel)
                    .Single();

                httpContext.Response.Cookies.Append(invalidModelChallenge.FlagKey, invalidModelChallenge.Flag);
            }

            if(_ctfOptions.CtfChallengeOptions.FreeCredit)
            {
                if (transactionTable.Amount < 0)
                {
                    if (transactionTable.ReceiverId == SecureBankConstants.CREDIT_USERNAME)
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if(transactionTable.Amount < 0)
                {
                    return false;
                }
            }

            if (_ctfOptions.CtfChallengeOptions.ExceptionHandlingTransactionCreate)
            {
                if (transactionTable.Id != 0)
                {
                    TransactionDBModel transaction = _transactionDAO.Get(transactionTable.Id);
                    if (transaction != null)
                    {
                        try
                        {
                            base.Create(transactionTable);
                        }
                        catch (Exception ex)
                        {
                            CtfChallengeModel exceptionHandlingChallenge = _ctfOptions.CtfChallenges
                                .Where(x => x.Type == CtfChallengeTypes.ExceptionHandling)
                                .Single();

                            throw new Exception(exceptionHandlingChallenge.Flag, ex);
                        }
                    }
                    else
                    {
                        transactionTable.Id = 0;
                    }
                }
            }

            return base.Create(transactionTable);
        }

        protected override bool CheckTransaction(TransactionDBModel transaction, HttpContext httpContext)
        {
            if(_ctfOptions.CtfChallengeOptions.FreeCredit)
            {
                if(transaction.ReceiverId == SecureBankConstants.CREDIT_USERNAME)
                {
                    CtfChallengeModel freeCredit = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.FreeCredit)
                        .SingleOrDefault();

                    httpContext.Response.Cookies.Append(freeCredit.FlagKey, freeCredit.Flag);

                    return true;
                }
            }

            return base.CheckTransaction(transaction, httpContext);
        }

        public override TransactionDBModel Details(int? id, HttpContext httpContext)
        {
            TransactionDBModel transaction = base.Details(id);
            if (transaction == null)
            {
                return null;
            }

            string userName = httpContext.GetUserName();
            string role = httpContext.GetRole();

            if (transaction.SenderId != userName && transaction.ReceiverId != userName && role != CookieConstants.ADMIN_ROLE_STRING)
            {
                if (_ctfOptions.CtfChallengeOptions.Enumeration)
                {
                    CtfChallengeModel enumerationChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.Enumeration)
                        .Single();

                    httpContext.Response.Headers.Add(enumerationChallenge.FlagKey, enumerationChallenge.Flag);
                }
                else
                {
                    return null;
                }
            }

            return transaction;
        }

        public override DataTableResp<TransactionResp> GetTransactions(string userName, string search, int start, int lenght, HttpContext httpContext)
        {
            List<TransactionResp> transactions;

            if (_ctfOptions.CtfChallengeOptions.SqlInjection)
            {
                CtfChallengeModel sqlInjectionChallenge = _ctfOptions.CtfChallenges
                    .Where(x => x.Type == CtfChallengeTypes.SqlInjection)
                    .Single();

                try
                {
                    transactions = _transactionDAO.GetTransactions(userName, search);
                }
                catch (Exception)
                {
                    httpContext.Response.Headers.Add(sqlInjectionChallenge.FlagKey, sqlInjectionChallenge.Flag);
                    throw;
                }

                List<TransactionResp> validTransactions = _transactionDAO.GetTransactionsCtfCheck(userName, search);

                if (validTransactions.Count != transactions.Count)
                {
                    httpContext.Response.Headers.Add(sqlInjectionChallenge.FlagKey, sqlInjectionChallenge.Flag);
                }
                else
                {
                    foreach (var transaction in transactions)
                    {
                        if (!validTransactions.Any(x => x.IsEqual(transaction)))
                        {
                            httpContext.Response.Headers.Add(sqlInjectionChallenge.FlagKey, sqlInjectionChallenge.Flag);
                            break;
                        }
                    }
                }
            }
            else
            {
                transactions = _transactionDAO.GetTransactionsCtfCheck(userName, search);
            }

            if (_ctfOptions.CtfChallengeOptions.TableXss)
            {
                bool xss = transactions.Any(x => CtfConstants.XXS_KEYVORDS.Any(c =>
                    (x.SenderId?.Contains(c) ?? false) || (x.ReceiverId?.Contains(c) ?? false) || (x.Reason?.Contains(c) ?? false) || (x.Reference?.Contains(c) ?? false)));
                if (xss)
                {
                    CtfChallengeModel xxsChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.Xss)
                        .Single();

                    httpContext.Response.Headers.Add(xxsChallenge.FlagKey, xxsChallenge.Flag);
                }
            }

            return new DataTableResp<TransactionResp>(
                recordsTotal: transactions.Count,
                recordsFiltered: transactions.Count,
                data: transactions
                    .Skip(start)
                    .Take(lenght)
                    .ToList());
        }

        public override List<TransactionsByDayResp> GetTransactionsByDay(string userName, HttpContext httpContext)
        {
            CtfChallengeModel sqlInjectionChallenge = _ctfOptions.CtfChallenges
                .Where(x => x.Type == CtfChallengeTypes.SqlInjection)
                .Single();

            List<TransactionsByDayResp> transactions;

            if (_ctfOptions.CtfChallengeOptions.SqlInjection)
            {
                try
                {
                    transactions = _transactionDAO.GetTransactionsByDay(userName);
                }
                catch (Exception)
                {
                    httpContext.Response.Headers.Add(sqlInjectionChallenge.FlagKey, sqlInjectionChallenge.Flag);
                    return null;
                }

                List<TransactionsByDayResp> validTransactions = _transactionDAO.GetTransactionsByDayCtfCheck(userName);

                if (validTransactions.Count != transactions.Count)
                {
                    httpContext.Response.Headers.Add(sqlInjectionChallenge.FlagKey, sqlInjectionChallenge.Flag);
                }
                else
                {
                    foreach (var transaction in transactions)
                    {
                        if (!validTransactions.Any(x => x.IsEqual(transaction)))
                        {
                            httpContext.Response.Headers.Add(sqlInjectionChallenge.FlagKey, sqlInjectionChallenge.Flag);
                            break;
                        }
                    }
                }
            }
            else
            {
                transactions = _transactionDAO.GetTransactionsByDayCtfCheck(userName);
            }

            return transactions;
        }

        public override string GetIndexViewName(HttpContext httpContext)
        {
            string user = httpContext.GetUserName();

            UserDBModel userDBModel = _userDAO.GetUser(user);
            if (!userDBModel.Confirmed)
            {
                if (_ctfOptions.CtfChallengeOptions.UnconfirmedLogin)
                {
                    CtfChallengeModel ctfChallengeModel = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.UnconfirmedLogin)
                        .SingleOrDefault();

                    // Cannot directly access ModelState from here, would need ActionContext
                    // This would require refactoring to pass it or handle differently
                }
            }

            return base.GetIndexViewName();
        }
    }
}
