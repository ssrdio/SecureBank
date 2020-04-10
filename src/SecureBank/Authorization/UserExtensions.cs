using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization
{
    public class UserExtensions : IUserExtensions
    {
        public string GetUserName(IPrincipal user)
        {
            Claim claim = ((ClaimsIdentity)user.Identity).FindFirst("userName");
            return claim?.Value;
        }

        public bool IsAuthenticated(IPrincipal user)
        {
            Claim claim = ((ClaimsIdentity)user.Identity).FindFirst("authenticated");
            return claim == null ? false : true;
        }
    }
}
