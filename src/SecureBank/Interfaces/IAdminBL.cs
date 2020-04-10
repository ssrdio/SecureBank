using SecureBank.Models;
using SecureBank.Models.Transaction;
using SecureBank.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IAdminBL
    {
        string GetIndexViewName();

        DataTableResp<TransactionResp> GetTransactions();
        DataTableResp<AdminUserInfoResp> GetUsers();
    }
}
