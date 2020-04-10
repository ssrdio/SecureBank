using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization.Attributes
{
    /// <summary>
    /// Only checks if user has Autheticatio cookie and sets claims.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthenticateAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IAuthorizeService authorizeService = (IAuthorizeService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizeService));
            authorizeService.AuthorizeNormal(context);
        }
    }
}
