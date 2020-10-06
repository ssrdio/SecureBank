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
        Task<bool> Register(UserModel userModel);

        Task<bool> ConfirmRegistration(string token);

        Task<UserModel> Login(UserModel loginModel);
        Task Logout(string returnUrl);

        Task<bool> PasswordRecovery(UserModel passwordRecoveryModel);
        Task<bool> RecoverPasswordValid(string token);
        Task<bool> RecoverPassword(UserModel passwordRecoveryModel);
        UserModel GetUser(string userName);

        IActionResult LoginAdmin();
    }
}
