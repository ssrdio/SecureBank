using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SecureBank.Helpers.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SecureBank.Helpers
{
    public static class UserExtended
    {
        public static bool IsAuthenticated(this HttpContext context)
        {
            IUserExtensions userExtensions = context.RequestServices.GetRequiredService<IUserExtensions>();

            return userExtensions.IsAuthenticated(context);
        }

        public static string GetUserName(this HttpContext context)
        {
            IUserExtensions userExtensions = context.RequestServices.GetRequiredService<IUserExtensions>();

            return userExtensions.GetUserName(context);
        }

        public static string GetRole(this HttpContext context)
        {
            IUserExtensions userExtensions = context.RequestServices.GetRequiredService<IUserExtensions>();

            return userExtensions.GetRole(context);
        }
    }
}
