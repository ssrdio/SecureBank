using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureBank.Services;

namespace SecureBank.Ctf.Services
{
    public class CtfAuthBL : AuthBL
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        private readonly CtfOptions _ctfOptions;

        public CtfAuthBL(IUserDAO userDAO, ITransactionDAO transactionDAO, IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor, IOptions<CtfOptions> options,
            IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor, IOptions<AppSettings> appSettings)
                : base(userDAO, transactionDAO, emailSender, httpContextAccessor, appSettings)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;

            _ctfOptions = options.Value;
        }

        public override string RegisterAdmin(UserModel userModel)
        {
            CtfChallangeModel hiddenPageChallange = _ctfOptions.CtfChallanges
                .Where(x => x.Type == CtfChallengeTypes.HiddenPage)
                .Single();

            return hiddenPageChallange.Flag;
        }

        public override Task<bool> Register(UserModel registrationModel)
        {
            if (registrationModel.Password.Length <= 3)
            {
                CtfChallangeModel weakPasswordChallenge = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.WeakPassword)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Headers.Add(weakPasswordChallenge.FlagKey, weakPasswordChallenge.Flag);
            }

            if (registrationModel.UserRight != 0)
            {
                CtfChallangeModel registrationRoleSetChallange = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.RegistrationRoleSet)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Headers.Add(registrationRoleSetChallange.FlagKey, registrationRoleSetChallange.Flag);
            }

            return base.Register(registrationModel);
        }

        public override Task Logout(string returnUrl)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            if (!urlHelper.IsLocalUrl(returnUrl))
            {
                CtfChallangeModel invalidRedirect = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.InvalidRedirect)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Cookies.Append(invalidRedirect.FlagKey, invalidRedirect.Flag);
            }

            return base.Logout(returnUrl);
        }
    }
}
