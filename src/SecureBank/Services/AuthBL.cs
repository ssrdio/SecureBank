using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NLog;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Auth;
using SecureBank.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Services
{
    public class AuthBL : IAuthBL
    {
        private const string AUTH_COOKIE = "SessionId";

        protected readonly IUserDAO _userDAO;
        protected readonly ITransactionDAO _transactionDAO;
        protected readonly IEmailSender _emailSender;

        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AppSettings _appSettings;

        protected readonly ILogger _accessLogger = LogManager.GetLogger("accessLogger");

        public AuthBL(IUserDAO userDAO, ITransactionDAO transactionDAO,  IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> appSettings)
        {
            _userDAO = userDAO;
            _transactionDAO = transactionDAO;

            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;

            _appSettings = appSettings.Value;
        }

        private string GetCurrentDomain()
        {
            return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        }

        public virtual async Task<bool> Register(UserModel registrationModel)
        {
            if (registrationModel == null || string.IsNullOrEmpty(registrationModel.UserName) || string.IsNullOrEmpty(registrationModel.Password))
            {
                return false;
            }

            bool registerUserResult = _userDAO.RegisterUser(registrationModel);
            if(!registerUserResult)
            {
                return false;
            }

            bool addWelcomeBonusResult = AddwelcomeBonus(registrationModel.UserName);

            string token = EncoderUtils.Base64Encode(registrationModel.UserName);

            string registrationMailSubject = "Confirm email";
            string registrationMailBody = $@"
                Hi! <br/>
                To confirm your email please follow the 
                <a href='{GetCurrentDomain()}/Auth/ConfirmRegistration?token={token}'>link</a> 
                <br/><br/><br/>
                Best regards!";

            if(_appSettings.IgnoreEmails)
            {
                return await ConfirmRegistration(token);
            }

            await _emailSender.SendEmailAsync(registrationModel.UserName, registrationMailSubject, registrationMailBody);

            return true;
        }

        private bool AddwelcomeBonus(string username)
        {
            DepositRequest depositRequest = new DepositRequest
            {
                SenderId = "SecureBank",
                ReceiverId = username,
                Amount = 10,
                Reason = "welcome bonus"
            };

            bool result = _transactionDAO.MakeDeposit(depositRequest);

            return result;
        }


        public virtual string RegisterAdmin(UserModel userModel)
        {
            return null;
        }

        public virtual Task<bool> ConfirmRegistration(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(false);
            }

            string userName = EncoderUtils.Base64Decode(token);

            bool result = _userDAO.ConfirmToken(userName);
            if (!result)
            {
                return Task.FromResult(false);
            }

            //_transactionDAO.MakeRandomTransactions(userName);
            return Task.FromResult(true);
        }


        public virtual Task<UserModel> Login(UserModel loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.UserName) || string.IsNullOrEmpty(loginModel.Password))
            {
                return Task.FromResult<UserModel>(null);
            }

            var accessLogModel = new
            {
                Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                Username = loginModel.UserName,
                Password = loginModel.Password
            };

            _accessLogger.Info($"{Newtonsoft.Json.JsonConvert.SerializeObject(accessLogModel)}");

            if (!_userDAO.ValidatePassword(loginModel.UserName, loginModel.Password))
            {
                return Task.FromResult<UserModel>(null);
            }

            UserDBModel userModel = _userDAO.GetUser(loginModel.UserName);

            Random random = new Random();

            var byteArray = new byte[256];
            random.NextBytes(byteArray);

            string cookie = Sha256HashUtils.ComputeSha256Hash(byteArray);
            string inrole = userModel.Role > 0 ? "1" : "0";

            if (userModel.Role > 50)
            {
                inrole = "100";
            }

            string allCookie = $"{EncoderUtils.Base64Encode(loginModel.UserName)}-{cookie}-{inrole}";

            if (!_userDAO.SaveSession(cookie, DateTime.UtcNow.AddDays(1)))
            {
                return Task.FromResult<UserModel>(null);
            }

            loginModel.Password = null;
            loginModel.Cookie = allCookie;
            loginModel.Status = "ok";

            _httpContextAccessor.HttpContext.Response.Cookies.Append(AUTH_COOKIE, loginModel.Cookie, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(3),
                HttpOnly = false
            });

            return Task.FromResult(loginModel);
        }

        public virtual Task Logout(string returnUrl)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(AUTH_COOKIE);

            return Task.CompletedTask;
        }


        public virtual async Task<bool> PasswordRecovery(UserModel passwordRecoveryModel)
        {
            if (passwordRecoveryModel == null || string.IsNullOrEmpty(passwordRecoveryModel.UserName))
            {
                return false;
            }

            if (!_userDAO.Exist(passwordRecoveryModel.UserName))
            {
                return false;
            }

            string passwordRecoverySubject = "Password recovery email";
            string passwordRecoveryBody = $@"
                Hi! <br/>
                You requested password recovery to complete request follow 
                <a href='{GetCurrentDomain()}/Auth/recover?token={EncoderUtils.Base64Encode(passwordRecoveryModel.UserName)}'>link</a>
                <br/><br/><br/>
                Best regards!";

            await _emailSender.SendEmailAsync(passwordRecoveryModel.UserName, passwordRecoverySubject, passwordRecoveryBody);

            return true;
        }
        public virtual Task<bool> RecoverPasswordValid(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(false);
            }

            string userName = EncoderUtils.Base64Decode(token);
            if (!_userDAO.Exist(userName))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
        public virtual Task<bool> RecoverPassword(UserModel passwordRecoveryModel)
        {
            if (passwordRecoveryModel == null || string.IsNullOrEmpty(passwordRecoveryModel.Token) || string.IsNullOrEmpty(passwordRecoveryModel.Password))
            {
                return Task.FromResult(false);
            }

            string userName = EncoderUtils.Base64Decode(passwordRecoveryModel.Token);
            if (!_userDAO.Exist(userName))
            {
                return Task.FromResult(false);
            }

            if (!_userDAO.UpdatePassword(userName, passwordRecoveryModel.Password))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
        public virtual UserModel GetUser(string userName)
        {
            UserDBModel user = _userDAO.GetUser(userName);
            return new UserModel
            {
                UserName = user.UserName,
                Password = user.Password
            };
        }
        public string GetLegalURL()
        {
            return _appSettings.LegalURL;
        }

        public bool IgnoreMails()
        {
            return _appSettings.IgnoreEmails;
        }



    }
}
