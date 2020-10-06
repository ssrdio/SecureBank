using SecureBank.DAL.DBModels;
using SecureBank.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IUserDAO
    {
        bool Exist(string userName);
        bool RegisterUser(UserModel userModel);
        bool ConfirmToken(string userName);
        bool UpdatePassword(string userName, string password);
        bool ValidatePassword(string userName, string pass, bool requireConfirmed);
        UserDBModel GetUser(string userName);
        bool SaveSession(string session, DateTime expires);
        bool ValidateSession(string session);
        List<UserDBModel> GetUsers();
        List<UserDBModel> GetUsers(string search);
        public bool UpdatePasswordToken(string userName, string token);
        public bool PasswordTokenExists(string token);
    }
}
