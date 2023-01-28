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
    public class CookieService : ICookieService
    {
        /// <summary>
        /// Users with role grater then this are admin
        /// </summary>
        protected const int ADMIN_ROLE = 50;

        protected const int COOKIE_PARTS = 3;

        protected const int USER_NAME_INDEX = 0;
        protected const int TOKEN_INDEX = 1;
        protected const int ROLE_INDEX = 2;

        protected const string COOKIE_KEY = "SessionId";

        protected const string ADMIN_ROLE_COOKIE_VALUE = "100";

        protected readonly TimeSpan COOKIE_VALID_FOR = new TimeSpan(1, 0, 0, 0);

        /// <summary>
        /// 0 = username
        /// 1 = cookie hash
        /// 2 = user role
        /// </summary>
        protected const string COOKIE_FORMAT = "{0}&{1}&{2}";
        protected const string COOKIE_SEPERATOR = "&";

        public CookieService()
        {
        }

        public virtual string CreateCookie(UserDBModel user, HttpContext context)
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

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.Add(COOKIE_VALID_FOR),
                HttpOnly = false,
            };

            context.Response.Cookies.Append(COOKIE_KEY, allCookie, cookieOptions);

            return allCookie;
        }

        public virtual void RemoveCookie(HttpContext context)
        {
            context.Response.Cookies.Delete(COOKIE_KEY);
        }

        public virtual IEnumerable<Claim> GetClaims(HttpContext context)
        {
            string cookie = context.Request.Cookies[COOKIE_KEY];

            if (string.IsNullOrEmpty(cookie))
            {
                return new List<Claim>();
            }

            return GetClaims(cookie);
        }

        public virtual bool ValidateCookie(HttpContext context)
        {
            string cookie = context.Request.Cookies[COOKIE_KEY];

            IUserDAO userDAO = context.RequestServices.GetRequiredService<IUserDAO>();

            return ValidateCookie(cookie, userDAO);
        }

        protected virtual bool ValidateCookie(string cookie, IUserDAO userDAO)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return false;
            }

            if (cookie.Split(COOKIE_SEPERATOR).Length != 3)
            {
                return false;
            }

            string cookieHash = GetCookieHash(cookie);

            bool isValid = userDAO.ValidateSession(cookieHash);
            if (!isValid)
            {
                return false;
            }

            return true;
        }

        protected virtual IEnumerable<Claim> GetClaims(string cookie)
        {
            Claim[] claims = new[]
            {
                new Claim(CookieConstants.AUTHENTICATED_CLAIM_TYPE, "True"),
                new Claim(CookieConstants.USERNAME_CLAIM_TYPE, EncoderUtils.Base64Decode(GetUserName(cookie))),
                new Claim(CookieConstants.ROLE_CLAIM_TYPE, GetRole(cookie)),
            };

            return claims;
        }

        protected virtual string GetRoleCookieValue()
        {
            return ADMIN_ROLE_COOKIE_VALUE;
        }

        protected virtual string GetUserName(string cookie)
        {
            string[] cookieParts = cookie.Split(COOKIE_SEPERATOR);

            return cookieParts[USER_NAME_INDEX];
        }

        protected virtual string GetCookieHash(string cookie)
        {
            string[] cookieParts = cookie.Split(COOKIE_SEPERATOR);

            return cookieParts[TOKEN_INDEX];
        }

        protected virtual string GetRole(string cookie)
        {
            string[] cookieParts = cookie.Split(COOKIE_SEPERATOR);

            bool canParseResult = int.TryParse(cookieParts[ROLE_INDEX], out int role);
            if(!canParseResult)
            {
                return CookieConstants.NORMAL_ROLE_STRING;
            }

            if(role > ADMIN_ROLE)
            {
                return CookieConstants.ADMIN_ROLE_STRING;
            }

            return CookieConstants.NORMAL_ROLE_STRING;
        }
    }
}
