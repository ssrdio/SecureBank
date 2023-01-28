using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf;
using SecureBank.Ctf.Models;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using SecureBank.Models.User;
using SecureBank.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.Services
{
    public class CtfAdminBL : AdminBL
    {
        private readonly CtfOptions _ctfOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CtfAdminBL(ITransactionDAO transactionDao, IUserDAO userDAO, IOptions<CtfOptions> ctfOptions, IHttpContextAccessor httpContextAccessor)
            : base(transactionDao, userDAO)
        {
            _ctfOptions = ctfOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public override DataTableResp<TransactionResp> GetTransactions()
        {
            DataTableResp<TransactionResp> transactions = base.GetTransactions();
            if (_ctfOptions.CtfChallengeOptions.TableXss)
            {
                bool xss = transactions.Data.Any(x => CtfConstants.XXS_KEYVORDS.Any(c =>
                    (x.SenderId?.Contains(c) ?? false) || (x.ReceiverId?.Contains(c) ?? false) || (x.Reason?.Contains(c) ?? false) || (x.Reference?.Contains(c) ?? false)
                    || (x.SenderName?.Contains(c) ?? false) || (x.SenderSurname?.Contains(c) ?? false) || (x.ReceiverName?.Contains(c) ?? false)
                    || (x.ReceiverSurname?.Contains(c) ?? false)));
                if (xss)
                {
                    CtfChallengeModel xssChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.Xss)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(xssChallenge.FlagKey, xssChallenge.Flag);
                }
            }
            return transactions;
        }

        public override DataTableResp<AdminUserInfoResp> GetUsers()
        {
            DataTableResp<AdminUserInfoResp> users = base.GetUsers();
            if (_ctfOptions.CtfChallengeOptions.TableXss)
            {
                bool xss = users.Data.Any(x => CtfConstants.XXS_KEYVORDS.Any(c =>
                    (x.Name?.Contains(c) ?? false) || (x.Surname?.Contains(c) ?? false) || (x.Username?.Contains(c) ?? false)));
                if (xss)
                {
                    CtfChallengeModel xxsChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.Xss)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(xxsChallenge.FlagKey, xxsChallenge.Flag);
                }
            }

            return users;
        }
    }
}
