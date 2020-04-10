using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Helpers.Authorization;
using SecureBank.Interfaces;
using System.Linq;

namespace SecureBank.Ctf.Authorization
{
    public class CtfAuthorizeService : AuthorizeService
    {
        public CtfAuthorizeService()
        {
        }

        public override bool AuthorizeAdmin(AuthorizationFilterContext context)
        {
            bool result = base.AuthorizeAdmin(context);
            if (!result)
            {
                return false;
            }

            string sessionId = context.HttpContext.Request.Cookies["SessionId"];
            string userName = EncoderUtils.Base64Decode(sessionId.Split("-")[USER_NAME_INDEX]);

            IUserDAO userDAO = context.HttpContext.RequestServices.GetRequiredService<IUserDAO>();

            UserDBModel user = userDAO.GetUser(userName);
            if (user.Role < ADMIN_ROLE)
            {
                CtfOptions ctfOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<CtfOptions>>().Value;

                CtfChallangeModel ctfChallange = ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.ChangeRoleInCookie)
                    .Single();

                context.HttpContext.Response.Headers.Add(ctfChallange.FlagKey, ctfChallange.Flag);
            }

            return true;
        }
    }
}
