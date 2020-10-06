using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization
{
    public interface IUserExtensions
    {
        bool IsAuthenticated(HttpContext context);
        string GetUserName(HttpContext context);
        string GetRole(HttpContext context);
    }
}
