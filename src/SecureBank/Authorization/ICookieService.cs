using Microsoft.AspNetCore.Http;
using SecureBank.DAL.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureBank.Authorization
{
    public interface ICookieService
    {
        string CreateCookie(UserDBModel user, HttpContext context);
        void RemoveCookie(HttpContext context);

        IEnumerable<Claim> GetClaims(HttpContext context);
        bool ValidateCookie(HttpContext context);
    }
}
