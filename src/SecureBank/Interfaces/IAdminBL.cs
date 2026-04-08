using SecureBank.Models;
using SecureBank.Models.Transaction;
using SecureBank.Models.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IAdminBL
    {
        string GetIndexViewName();

        DataTableResp<TransactionResp> GetTransactions(HttpContext httpContext = null);
        DataTableResp<AdminUserInfoResp> GetUsers(HttpContext httpContext = null);
    }
}
