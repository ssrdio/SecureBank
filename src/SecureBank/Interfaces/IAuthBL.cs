using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IAuthBL
    {
        bool IgnoreMails();
        Task<bool> Register(UserModel userModel, HttpContext httpContext);

        Task<bool> ConfirmRegistration(string token);

        Task<UserModel> Login(UserModel loginModel, HttpContext httpContext);
        Task Logout(string returnUrl, HttpContext httpContext);

        Task<bool> PasswordRecovery(UserModel passwordRecoveryModel);
        Task<bool> RecoverPasswordValid(string token);
        Task<bool> RecoverPassword(UserModel passwordRecoveryModel);
        UserModel GetUser(string userName);

        IActionResult LoginAdmin();
    }
}
