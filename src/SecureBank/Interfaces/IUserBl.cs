using SecureBank.Models.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IUserBL
    {
        AccountBalanceResp GetAmount(string userName, HttpContext httpContext = null);
        byte[] GetProfileImage(string userName, HttpContext httpContext = null);

        List<string> FindUsers(string search);
        Task<byte[]> SetProfileImageUrl(string username, string url);
    }
}
