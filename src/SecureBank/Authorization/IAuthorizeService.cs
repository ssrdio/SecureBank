using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization
{
    public interface IAuthorizeService
    {
        bool AuthorizeAdmin(AuthorizationFilterContext context);
        bool AuthorizeNormal(AuthorizationFilterContext context);
        bool AuthorizeMissing(AuthorizationFilterContext context);
    }
}
