using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models.User;
using SecureBank.Services;
using SecureBank.Authorization;
using System.IO;

namespace SecureBank.Ctf.Services
{
    public class CtfUserBL : UserBL
    {
        private readonly string[] CTF_FILES = new string[]
        {
            "../appsettings.json",
            "../../appsettings.json",
            "..\\appsettings.json",
            "..\\..\\appsettings.json"
        };

        private readonly ICookieService _cookieService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly CtfOptions _ctfOptions;

        public CtfUserBL(
            ITransactionDAO transactionDAO,
            IWebHostEnvironment webHostEnvironment,
            IUserDAO userDAO,
            ICookieService cookieService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<CtfOptions> ctfOptions) : base(transactionDAO, userDAO, webHostEnvironment)
        {
            _cookieService = cookieService;
            _httpContextAccessor = httpContextAccessor;

            _ctfOptions = ctfOptions.Value;
        }

        public override AccountBalanceResp GetAmount(string userName)
        {
            string loggedInUserName = _httpContextAccessor.HttpContext.GetUserName();

            if (_ctfOptions.CtfChallengeOptions.SensitiveDataExposureBalance)
            {
                if(userName != loggedInUserName)
                {
                    CtfChallengeModel sensitiveDataExposure = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.SensitiveDataExposure)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(sensitiveDataExposure.FlagKey, sensitiveDataExposure.Flag);
                }
            }
            else
            {
                userName = loggedInUserName;
            }

            return base.GetAmount(userName);
        }

        public override byte[] GetProfileImage(string userName)
        {
            string loggedInUserName = _httpContextAccessor.HttpContext.GetUserName();

            if (_ctfOptions.CtfChallengeOptions.PathTraversal)
            {
                if (userName != null && CTF_FILES.Contains(userName))
                {
                    CtfChallengeModel pathTraversal = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.PathTraversal)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(pathTraversal.FlagKey, pathTraversal.Flag);
                }
            }
            else
            {
                foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
                {
                    userName = userName.Replace(invalidCharacter, '_');
                }
            }

            if (_ctfOptions.CtfChallengeOptions.SensitiveDataExposureProfileImage)
            {
                if (userName != loggedInUserName)
                {
                    CtfChallengeModel sensitiveDataExposure = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.SensitiveDataExposure)
                        .Single();

                    _httpContextAccessor.HttpContext.Response.Headers.Add(sensitiveDataExposure.FlagKey, sensitiveDataExposure.Flag);
                }
            }
            else
            {
                userName = loggedInUserName;
            }

            return base.GetProfileImage(userName);
        }
    }
}
