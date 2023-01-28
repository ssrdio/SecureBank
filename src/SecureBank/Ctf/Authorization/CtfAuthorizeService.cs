using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecureBank.Authorization;
using SecureBank.Ctf.Models;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SecureBank.Ctf.Authorization
{
    public class CtfAuthorizeService : AuthorizeService
    {
        private readonly CtfOptions _ctfOptions;

        public CtfAuthorizeService(ICookieService cookieService, IOptions<CtfOptions> ctfOptions) : base(cookieService)
        {
            _ctfOptions = ctfOptions.Value;
        }

        public override bool AuthorizeAdmin(AuthorizationFilterContext context)
        {
            bool result = base.AuthorizeAdmin(context);
            if (!result)
            {
                return false;
            }

            IEnumerable<Claim> claims = _cookieService.GetClaims(context.HttpContext);

            string userName = claims
                .Where(x => x.Type == CookieConstants.USERNAME_CLAIM_TYPE)
                .Select(x => x.Value)
                .SingleOrDefault();

            IUserDAO userDAO = context.HttpContext.RequestServices.GetRequiredService<IUserDAO>();

            UserDBModel user = userDAO.GetUser(userName);

            if (_ctfOptions.CtfChallengeOptions.ChangeRoleInCookie)
            {
                if (user.Role < 50)
                {
                    CtfOptions ctfOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<CtfOptions>>().Value;

                    CtfChallengeModel ctfChallenge = ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.ChangeRoleInCookie)
                        .Single();

                    context.HttpContext.Response.Headers.Add(ctfChallenge.FlagKey, ctfChallenge.Flag);
                }
            }
            else
            {
                if(user.Role < 50)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool AuthorizeMissing(AuthorizationFilterContext context)
        {
            CtfOptions ctfOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<CtfOptions>>().Value;

            ICookieService cookieService = context.HttpContext.RequestServices.GetRequiredService<ICookieService>();
            bool isCookieValid = cookieService.ValidateCookie(context.HttpContext);

            if (ctfOptions.CtfChallengeOptions.MissingAuthentication)
            {
                CtfChallengeModel missingAuth = ctfOptions.CtfChallenges
                    .Where(x => x.Type == CtfChallengeTypes.MissingAuthentication)
                    .Single();

                context.HttpContext.Response.Headers.Add(missingAuth.FlagKey, missingAuth.Flag);
            }
            else
            {
                if (!isCookieValid)
                {
                    return false;
                }
                else
                {
                    IEnumerable<Claim> claims = _cookieService.GetClaims(context.HttpContext);
                    if (claims == null)
                    {
                        return false;
                    }

                    GenericPrincipal tmpUser = new GenericPrincipal(new ClaimsIdentity(claims), Array.Empty<string>());

                    context.HttpContext.User = tmpUser;
                }    
            }

            return true;
        }
    }
}
