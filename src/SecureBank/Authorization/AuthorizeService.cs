using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SecureBank.Helpers.Authorization;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SecureBank.Authorization
{
    public class AuthorizeService : IAuthorizeService
    {
        protected readonly ICookieService _cookieService;

        public AuthorizeService(ICookieService cookieService)
        {
            _cookieService = cookieService;
        }

        public virtual bool AuthorizeAdmin(AuthorizationFilterContext context)
        {
            bool isCookieValid = _cookieService.ValidateCookie(context.HttpContext);
            if (!isCookieValid)
            {
                return false;
            }

            IEnumerable<Claim> claims = _cookieService.GetClaims(context.HttpContext);
            if (claims == null)
            {
                return false;
            }

            Claim roleClaim = claims
                .Where(x => x.Type == CookieConstants.ROLE_CLAIM_TYPE)
                .SingleOrDefault();
            if (roleClaim == null)
            {
                return false;
            }

            if (roleClaim.Value != CookieConstants.ADMIN_ROLE_STRING)
            {
                return false;
            }

            GenericPrincipal tmpUser = new GenericPrincipal(new ClaimsIdentity(claims), Array.Empty<string>());

            context.HttpContext.User = tmpUser;

            return true;
        }

        public virtual bool AuthorizeMissing(AuthorizationFilterContext context)
        {
            return true;
        }

        public virtual bool AuthorizeNormal(AuthorizationFilterContext context)
        {
            bool isCookieValid = _cookieService.ValidateCookie(context.HttpContext);
            if (!isCookieValid)
            {
                return false;
            }

            IEnumerable<Claim> claims = _cookieService.GetClaims(context.HttpContext);
            if (claims == null)
            {
                return false;
            }

            GenericPrincipal tmpUser = new GenericPrincipal(new ClaimsIdentity(claims), Array.Empty<string>());

            context.HttpContext.User = tmpUser;

            return true;
        }
    }
}
