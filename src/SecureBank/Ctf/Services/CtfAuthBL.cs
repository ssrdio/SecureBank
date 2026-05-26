using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly CtfOptions _ctfOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Vulnerable regex pattern for reDOS challenge - catastrophic backtracking on non-email strings
        private const string REDOS_EMAIL_REGEX_PATTERN =
            @"^([a-zA-Z0-9]+([._-]?[a-zA-Z0-9]+)*)+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public CtfAuthBL(
            IUserDAO userDAO,
            ITransactionDAO transactionDAO,
            IEmailSender emailSender,
            IOptions<CtfOptions> options,
            ICookieService cookieService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor)
            : base(userDAO, transactionDAO, emailSender, cookieService, appSettings)
        {

            _ctfOptions = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<bool> Register(UserModel registrationModel, HttpContext httpContext)
        {
            if (registrationModel.Password.Length <= 3)
            {
                if (_ctfOptions.CtfChallengeOptions.WeakPassword)
                {
                    CtfChallengeModel weakPasswordChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.WeakPassword)
                        .Single();

                    httpContext.Response.Headers[weakPasswordChallenge.FlagKey] = weakPasswordChallenge.Flag;
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

                    httpContext.Response.Headers[registrationRoleSetChallenge.FlagKey] = registrationRoleSetChallenge.Flag;
                }
                else
                {
                    registrationModel.UserRight = 0;
                }
            }

            return base.Register(registrationModel, httpContext);
        }

        protected override bool ValidateRegistrationModel(UserModel userModel)
        {
            if (userModel == null || string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
            {
                return false;
            }

            string pattern = _ctfOptions.CtfChallengeOptions.reDOS ? REDOS_EMAIL_REGEX_PATTERN : EMAIL_REGEX_PATTERN;

            try
            {
                bool isEmailValid = Regex.IsMatch(userModel.UserName, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                if (!isEmailValid)
                {
                    return false;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                if (_ctfOptions.CtfChallengeOptions.reDOS)
                {
                    CtfChallengeModel reDosChallenge = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.reDOS)
                        .Single();

                    _httpContextAccessor.HttpContext?.Response.Headers[reDosChallenge.FlagKey] = reDosChallenge.Flag;
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

        public override Task Logout(string returnUrl, HttpContext httpContext)
        {
            if (!IsLocalUrl(returnUrl))
            {
                if (_ctfOptions.CtfChallengeOptions.InvalidRedirect)
                {
                    CtfChallengeModel invalidRedirect = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.InvalidRedirect)
                        .Single();

                    httpContext.Response.Cookies.Append(invalidRedirect.FlagKey, invalidRedirect.Flag);
                }
                else
                {
                    returnUrl = "/";
                }
            }

            return base.Logout(returnUrl, httpContext);
        }

        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            // Relative URLs are local
            if (url[0] == '/')
                return true;

            // Check if it's a relative URL (doesn't start with protocol)
            if (!url.Contains("://"))
                return true;

            return false;
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
