using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SecureBank.Ctf.Models;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models.User;
using SecureBank.Services;

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

        private readonly IUserDAO _userDAO;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly CtfOptions _ctfOptions;

        public CtfUserBL(ITransactionDAO transactionDAO, IWebHostEnvironment webHostEnvironment, IUserDAO userDAO,
            IHttpContextAccessor httpContextAccessor, IOptions<CtfOptions> ctfOptions) : base(transactionDAO, userDAO, webHostEnvironment)
        {
            _userDAO = userDAO;

            _httpContextAccessor = httpContextAccessor;

            _ctfOptions = ctfOptions.Value;
        }

        private void ValidateBrokenAuthSensitivedataExposure(string userName)
        {
            string sessionId = _httpContextAccessor.HttpContext.Request.Cookies["SessionId"];
            string logedInUser = null;

            CtfChallangeModel missingAuthChallange = _ctfOptions.CtfChallanges
                .Where(x => x.Type == CtfChallengeTypes.MissingAuthentication)
                .Single();

            if (string.IsNullOrEmpty(sessionId))
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add(missingAuthChallange.FlagKey, missingAuthChallange.Flag);
            }
            else
            {
                if (!_userDAO.ValidateSession(sessionId.Split("-")[1]))
                {
                    _httpContextAccessor.HttpContext.Response.Headers.Add(missingAuthChallange.FlagKey, missingAuthChallange.Flag);
                }

                logedInUser = EncoderUtils.Base64Decode(sessionId.Split("-")[0]);
            }

            if (logedInUser != userName)
            {
                CtfChallangeModel sensitiveDataExposureChallenge = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.SensitiveDataExposure)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Headers.Add(sensitiveDataExposureChallenge.FlagKey, sensitiveDataExposureChallenge.Flag);
            }
        }

        public override AccountBalanceResp GetAmount(string userName)
        {
            ValidateBrokenAuthSensitivedataExposure(userName);

            return base.GetAmount(userName);
        }

        public override byte[] GetProfileImage(string userName)
        {
            ValidateBrokenAuthSensitivedataExposure(userName);

            byte[] profileImage;

            try
            {
                profileImage = base.GetProfileImage(userName);
            }
            catch(Exception ex)
            {
                CtfChallangeModel exceptionHandlingChallange = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.ExcaptionHandling)
                    .Single();

                throw new Exception(exceptionHandlingChallange.Flag, ex);
            }

            if (userName != null && CTF_FILES.Contains(userName))
            {
                CtfChallangeModel pathTraversal = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.PathTraversal)
                    .Single();

                _httpContextAccessor.HttpContext.Response.Headers.Add(pathTraversal.FlagKey, pathTraversal.Flag);
            }

            return profileImage;
        }
    }
}
