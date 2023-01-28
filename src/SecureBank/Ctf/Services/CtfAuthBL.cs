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
using SecureBank.Authorization;
using System.Text.RegularExpressions;

namespace SecureBank.Ctf.Services
{
    public class CtfAuthBL : AuthBL
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        private readonly CtfOptions _ctfOptions;

        public CtfAuthBL(
            IUserDAO userDAO,
            ITransactionDAO transactionDAO,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor,
            IOptions<CtfOptions> options,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            ICookieService cookieService,
            IOptions<AppSettings> appSettings)
            : base(userDAO, transactionDAO, emailSender, cookieService, httpContextAccessor, appSettings)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;

            _ctfOptions = options.Value;
        }

        public override Task<bool> Register(UserModel registrationModel)
        {
            if (registrationModel.Password.Length <= 3)
            {
                if (_ctfOptions.CtfChallengeOptions.WeakPassword)
                {
                    CtfChallengeModel weakPasswordChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.WeakPassword)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(weakPasswordChallenge.FlagKey, weakPasswordChallenge.Flag);
                }
                else
                {
                    return Task.FromResult(false);
                }
            }

            if (registrationModel.UserRight != 0)
            {
                if (_ctfOptions.CtfChallengeOptions.RegistrationRoleSet)
                {
                    CtfChallengeModel registrationRoleSetChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.RegistrationRoleSet)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(registrationRoleSetChallenge.FlagKey, registrationRoleSetChallenge.Flag);
                }
                else
                {
                    registrationModel.UserRight = 0;
                }
            }

            return base.Register(registrationModel);
        }

        protected override bool ValidateRegistrationModel(UserModel userModel)
        {
            if (userModel == null || string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
            {
                return false;
            }

            try
            {
                bool isEmailValid = Regex.IsMatch(userModel.UserName, EMAIL_REGEX_PATTERN, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                if (!isEmailValid)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                if (_ctfOptions.CtfChallengeOptions.reDOS)
                {
                    CtfChallengeModel reDOS = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.reDOS)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(reDOS.FlagKey, reDOS.Flag);
                }

                return false;
            }

            return true;
        }

        protected override bool ValidatePassword(UserModel userModel)
        {
            if(_ctfOptions.CtfChallengeOptions.UnconfirmedLogin)
            {
                return base.ValidatePassword(userModel);
            }

            return _userDAO.ValidatePassword(userModel.UserName, userModel.Password, true);
        }

        public override Task Logout(string returnUrl)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            if (!urlHelper.IsLocalUrl(returnUrl))
            {
                if (_ctfOptions.CtfChallengeOptions.InvalidRedirect)
                {
                    CtfChallengeModel invalidRedirect = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.InvalidRedirect)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Cookies.Append(invalidRedirect.FlagKey, invalidRedirect.Flag);
                }
                else
                {
                    returnUrl = "/";
                }
            }

            return base.Logout(returnUrl);
        }

        public override IActionResult LoginAdmin()
        {
            if (!_ctfOptions.CtfChallengeOptions.HiddenPageLoginAdmin)
            {
                return base.LoginAdmin();
            }


            CtfChallengeModel hiddenPageModel = _ctfOptions.CtfChallenges
                .Where(x => x.Type == CtfChallengeTypes.HiddenPage)
                .SingleOrDefault();

            return new OkObjectResult(new { Flag = hiddenPageModel.Flag });
        }
    }
}
