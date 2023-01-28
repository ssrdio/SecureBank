using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecureBank.Authorization
{
    public class EncryptedCookieService : CookieService
    {
        /// <summary>
        /// Users with role grater then this are admin
        /// </summary>

        protected const string PROTECTOR_PURPOSE = "AuthCookie";

        protected new const string ADMIN_ROLE_COOKIE_VALUE = "331d014a-412e-4671-9c0f-5c5653b21848";

        private readonly IDataProtector _protector;

        public EncryptedCookieService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector(PROTECTOR_PURPOSE);
        }

        public override string CreateCookie(UserDBModel user, HttpContext context)
        {
            Random random = new Random();

            var byteArray = new byte[256];
            random.NextBytes(byteArray);

            string cookieHash = Sha256HashUtils.ComputeSha256Hash(byteArray);
            string inRole = user.Role.ToString();

            if (user.Role > ADMIN_ROLE)
            {
                inRole = ADMIN_ROLE_COOKIE_VALUE;
            }

            IUserDAO userDAO = context.RequestServices.GetRequiredService<IUserDAO>();
            bool saveSessionResult = userDAO.SaveSession(cookieHash, DateTime.UtcNow.Add(COOKIE_VALID_FOR));
            if (!saveSessionResult)
            {
                return null;
            }

            string allCookie = string.Format(COOKIE_FORMAT, EncoderUtils.Base64Encode(user.UserName), cookieHash, inRole);

            string encodedCookie = _protector.Protect(allCookie);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1)
            };

            context.Response.Cookies.Append(COOKIE_KEY, encodedCookie, cookieOptions);

            return encodedCookie;
        }

        public override IEnumerable<Claim> GetClaims(HttpContext context)
        {
            string cookie = context.Request.Cookies[COOKIE_KEY];

            if (string.IsNullOrEmpty(cookie))
            {
                return null;
            }

            string decodedCookie;

            try
            {
                decodedCookie = _protector.Unprotect(cookie);
            }
            catch(Exception)
            {
                return null;
            }

            return GetClaims(decodedCookie);
        }

        public override bool ValidateCookie(HttpContext context)
        {
            string cookie = context.Request.Cookies[COOKIE_KEY];

            IUserDAO userDAO = context.RequestServices.GetRequiredService<IUserDAO>();

            string decodedCookie;

            try
            {
                decodedCookie = _protector.Unprotect(cookie);
            }
            catch (Exception)
            {
                return false;
            }

            return ValidateCookie(decodedCookie, userDAO);
        }

        protected override string GetRoleCookieValue()
        {
            return ADMIN_ROLE_COOKIE_VALUE;
        }

        protected override string GetRole(string cookie)
        {
            string role = cookie.Split(COOKIE_SEPERATOR)[ROLE_INDEX];

            if(role == ADMIN_ROLE_COOKIE_VALUE)
            {
                return CookieConstants.ADMIN_ROLE_STRING;
            }

            return CookieConstants.NORMAL_ROLE_STRING;
        }
    }
}
