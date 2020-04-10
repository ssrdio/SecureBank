using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeMissing : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IAuthorizeService authorizeService = (IAuthorizeService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizeService));
            bool result = authorizeService.AuthorizeMissing(context);
        }
    }
}
