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

        private readonly CtfOptions _ctfOptions;

        public CtfUserBL(
            ITransactionDAO transactionDAO,
            IWebHostEnvironment webHostEnvironment,
            IUserDAO userDAO,
            ICookieService cookieService,
            IOptions<CtfOptions> ctfOptions) : base(transactionDAO, userDAO, webHostEnvironment)
        {
            _cookieService = cookieService;

            _ctfOptions = ctfOptions.Value;
        }

        public override AccountBalanceResp GetAmount(string userName, HttpContext httpContext)
        {
            string loggedInUserName = httpContext.GetUserName();

            if (_ctfOptions.CtfChallengeOptions.SensitiveDataExposureBalance)
            {
                if(userName != loggedInUserName)
                {
                    CtfChallengeModel sensitiveDataExposure = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.SensitiveDataExposure)
                        .Single();

                    httpContext.Response.Headers.Add(sensitiveDataExposure.FlagKey, sensitiveDataExposure.Flag);
                }
            }
            else
            {
                userName = loggedInUserName;
            }

            return base.GetAmount(userName, httpContext);
        }

        public override byte[] GetProfileImage(string userName, HttpContext httpContext)
        {
            string loggedInUserName = httpContext.GetUserName();

            if (_ctfOptions.CtfChallengeOptions.PathTraversal)
            {
                if (userName != null && CTF_FILES.Contains(userName))
                {
                    CtfChallengeModel pathTraversal = _ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.PathTraversal)
                        .Single();

                    httpContext.Response.Headers.Add(pathTraversal.FlagKey, pathTraversal.Flag);
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

                    httpContext.Response.Headers.Add(sensitiveDataExposure.FlagKey, sensitiveDataExposure.Flag);
                }
            }
            else
            {
                userName = loggedInUserName;
            }

            return base.GetProfileImage(userName, httpContext);
        }
    }
}
