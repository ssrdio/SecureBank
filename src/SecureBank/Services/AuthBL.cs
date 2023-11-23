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
using System.Threading.Tasks;
using SecureBank.Authorization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SecureBank.Services
{
    public class AuthBL : IAuthBL
    {
        protected readonly IUserDAO _userDAO;
        protected readonly ITransactionDAO _transactionDAO;
        protected readonly IEmailSender _emailSender;
        protected readonly ICookieService _cookieService;

        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly AppSettings _appSettings;

        protected readonly ILogger _accessLogger = LogManager.GetLogger("accessLogger");

        protected const string EMAIL_REGEX_PATTERN =
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public AuthBL(
            IUserDAO userDAO,
            ITransactionDAO transactionDAO,
            IEmailSender emailSender,
            ICookieService cookieService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettings> appSettings)
        {
            _userDAO = userDAO;
            _transactionDAO = transactionDAO;

            _cookieService = cookieService;

            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;

            _appSettings = appSettings.Value;
        }
        private string ContextScheme =>
            _httpContextAccessor.HttpContext.Request.Scheme;
        private string ContextHost =>
            _httpContextAccessor.HttpContext.Request.Host.ToString();
        private string GetCurrentDomain =>
            $"{ContextScheme}://{ContextHost}";


        public virtual async Task<bool> Register(UserModel registrationModel)
        {
            bool isModelValid = ValidateRegistrationModel(registrationModel);
            if (!isModelValid)
            {
                return false;
            }

            bool registerUserResult = _userDAO.RegisterUser(registrationModel);
            if (!registerUserResult)
            {
                return false;
            }

            bool addWelcomeBonusResult = AddWelcomeBonus(registrationModel.UserName);
            if(addWelcomeBonusResult)
            {
                _accessLogger.Info($"Giving welcome bonus to {registrationModel.UserName}");
            }

            string token = EncoderUtils.Base64Encode(registrationModel.UserName);

            string baseUrl;
            if (!string.IsNullOrEmpty(_appSettings.BaseUrl))
            {
                baseUrl = _appSettings.BaseUrl;
            }
            else
            {
                baseUrl = GetCurrentDomain;
            }

            string registrationMailSubject = "Confirm email";
            string registrationMailBody = $@"
                Hi! <br/>
                To confirm your email please follow the 
                <a href='{baseUrl}/Auth/ConfirmRegistration?token={token}'>link</a> 
                <br/><br/><br/>
                Best regards!";

            if (_appSettings.IgnoreEmails)
            {
                return await ConfirmRegistration(token);
            }

            await _emailSender.SendEmailAsync(
                registrationModel.UserName,
                registrationMailSubject,
                registrationMailBody);

            return true;
        }

        protected virtual bool ValidateRegistrationModel(UserModel userModel)
        {
            if (userModel == null ||
                string.IsNullOrEmpty(userModel.UserName) ||
                string.IsNullOrEmpty(userModel.Password))
            {
                return false;
            }

            bool isEmailValid = Regex.IsMatch(userModel.UserName, EMAIL_REGEX_PATTERN);
            if (!isEmailValid)
            {
                return false;
            }

            return true;
        }

        private bool AddWelcomeBonus(string username)
        {
            DepositRequest depositRequest = new DepositRequest
            {
                SenderId = "SecureBank",
                ReceiverId = username,
                Amount = 100,
                Reason = "welcome bonus"
            };

            bool result = _transactionDAO.MakeDeposit(depositRequest);

            return result;
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
            if (loginModel == null ||
                string.IsNullOrEmpty(loginModel.UserName) ||
                string.IsNullOrEmpty(loginModel.Password))
            {
                return Task.FromResult<UserModel>(null);
            }

            var accessLogModel = new
            {
                Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                Username = loginModel.UserName,
                Password = loginModel.Password
            };

            _accessLogger.Info($"{JsonConvert.SerializeObject(accessLogModel)}");

            bool passwordValid = ValidatePassword(loginModel);
            if (!passwordValid)
            {
                return Task.FromResult<UserModel>(null);
            }

            UserDBModel userModel = _userDAO.GetUser(loginModel.UserName);

            string cookie = _cookieService.CreateCookie(userModel, _httpContextAccessor.HttpContext);
            if (string.IsNullOrEmpty(cookie))
            {
                return Task.FromResult<UserModel>(null);
            }

            loginModel.Password = null;
            loginModel.Cookie = cookie;
            loginModel.Status = "ok";

            return Task.FromResult(loginModel);
        }

        protected virtual bool ValidatePassword(UserModel userModel)
        {
            return _userDAO.ValidatePassword(userModel.UserName, userModel.Password, false);
        }

        public virtual Task Logout(string returnUrl)
        {
            _cookieService.RemoveCookie(_httpContextAccessor.HttpContext);

            return Task.CompletedTask;
        }


        public virtual async Task<bool> PasswordRecovery(UserModel passwordRecoveryModel)
        {
            if (passwordRecoveryModel == null ||
                string.IsNullOrEmpty(passwordRecoveryModel.UserName))
            {
                return false;
            }

            if (!_userDAO.Exist(passwordRecoveryModel.UserName))
            {
                return false;
            }
            string token = new Guid().ToString();
            _userDAO.UpdatePasswordToken(passwordRecoveryModel.UserName, token);

            string passwordRecoverySubject = "Password recovery email";
            string passwordRecoveryFollow =
                $"{_appSettings.BaseUrl}/Auth/recover?token={EncoderUtils.Base64Encode(token)}";
            string passwordRecoveryBody = $@"
                Hi! <br/>
                You requested password recovery to complete request follow 
                <a href='{passwordRecoveryFollow}'>link</a>
                <br/><br/><br/>
                Best regards!";

            await _emailSender.SendEmailAsync(
                passwordRecoveryModel.UserName,
                passwordRecoverySubject,
                passwordRecoveryBody);

            return true;
        }
        public virtual Task<bool> RecoverPasswordValid(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(false);
            }

            string userName = EncoderUtils.Base64Decode(token);
            if (!_userDAO.PasswordTokenExists(userName))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
        public virtual Task<bool> RecoverPassword(UserModel passwordRecoveryModel)
        {
            if (passwordRecoveryModel == null ||
                string.IsNullOrEmpty(passwordRecoveryModel.Token) ||
                string.IsNullOrEmpty(passwordRecoveryModel.Password))
            {
                return Task.FromResult(false);
            }

            string userName = EncoderUtils.Base64Decode(passwordRecoveryModel.Token);
            if (!_userDAO.PasswordTokenExists(userName))
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

        public bool IgnoreMails()
        {
            return _appSettings.IgnoreEmails;
        }

        public virtual IActionResult LoginAdmin()
        {
            return new NotFoundResult();
        }
    }
}
