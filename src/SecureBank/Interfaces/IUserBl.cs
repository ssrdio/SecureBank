using SecureBank.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IUserBL
    {
        AccountBalanceResp GetAmount(string userName);
        byte[] GetProfileImage(string userName);

        List<string> FindUsers(string search);
        Task<bool> SetProfileImageUrl(string username, string url);
    }
}
