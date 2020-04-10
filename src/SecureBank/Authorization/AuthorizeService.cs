using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SecureBank.Helpers.Authorization
{
    public class AuthorizeService : IAuthorizeService
    {
        /// <summary>
        /// Users with role grater then this are admin
        /// </summary>
        protected const int ADMIN_ROLE = 50;

        protected const int COOKIE_PARTS = 3;

        protected const int USER_NAME_INDEX = 0;
        protected const int TOKEN_INDEX = 1;
        protected const int ROLDE_INDEX = 2;

        public AuthorizeService()
        {
        }

        public virtual bool AuthorizeAdmin(AuthorizationFilterContext context)
        {
            string sessionId = context.HttpContext.Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            string[] sessionParts = sessionId.Split('-');
            if (sessionParts.Length != COOKIE_PARTS)
            {
                return false;
            }

            IUserDAO userDAO = context.HttpContext.RequestServices.GetRequiredService<IUserDAO>();
            if (userDAO.ValidateSession(sessionParts[TOKEN_INDEX]) == false)
            {
                return false;
            }

            bool parseRoleResult = int.TryParse(sessionParts[ROLDE_INDEX], out int role);
            if (!parseRoleResult)
            {
                return false;
            }

            if (role < ADMIN_ROLE)
            {
                return false;
            }

            Claim[] claims = new[]
            {
                new Claim("authenticated", "true"),
                new Claim("userName", EncoderUtils.Base64Decode(sessionParts[USER_NAME_INDEX]))
            };

            GenericPrincipal tmpUser = new GenericPrincipal(new ClaimsIdentity(claims), Array.Empty<string>());

            context.HttpContext.User = tmpUser;

            return true;
        }

        public virtual bool AuthorizeMissing(AuthorizationFilterContext context)
        {
            return true;
        }

        public virtual bool AuthorizeNormal(AuthorizationFilterContext context)
        {
            string sessionId = context.HttpContext.Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            string[] sessionParts = sessionId.Split('-');
            if (sessionParts.Length != COOKIE_PARTS)
            {
                return false;
            }

            IUserDAO userDAO = context.HttpContext.RequestServices.GetRequiredService<IUserDAO>();
            if (userDAO.ValidateSession(sessionParts[TOKEN_INDEX]) == false)
            {
                return false;
            }

            Claim[] claims = new[]
            {
                new Claim("authenticated", "true"),
                new Claim("userName", EncoderUtils.Base64Decode(sessionParts[USER_NAME_INDEX]))
            };

            GenericPrincipal tmpUser = new GenericPrincipal(new ClaimsIdentity(claims), Array.Empty<string>());

            context.HttpContext.User = tmpUser;

            return true;
        }
    }
}
