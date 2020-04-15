using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization
{
    public interface IUserExtensions
    {
        bool IsAuthenticated(IPrincipal user);
        string GetUserName(IPrincipal user);
        string GetRole(IPrincipal user);
    }
}
