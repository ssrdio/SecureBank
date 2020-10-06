using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace SecureBank.Helpers.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeMissing : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly AuthorizeAttributeTypes _authorizeAttributeType;

        public AuthorizeMissing(AuthorizeAttributeTypes authorizeAttributeTypes)
        {
            _authorizeAttributeType = authorizeAttributeTypes;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IAuthorizeService authorizeService = (IAuthorizeService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizeService));
            bool result = authorizeService.AuthorizeMissing(context);

            if (!result)
            {
                switch (_authorizeAttributeType)
                {
                    case AuthorizeAttributeTypes.Mvc:
                        {
                            context.Result = new RedirectToActionResult("Login", "Auth", null);
                            return;
                        }
                    case AuthorizeAttributeTypes.Api:
                        {

                            context.Result = new UnauthorizedResult();
                            return;
                        }
                    default:
                        {
                            throw new Exception($"Unsupported AuthorizeAttributeType");
                        }
                }
            }
        }
    }
}
