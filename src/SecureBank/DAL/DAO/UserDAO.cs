using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using SecureBank.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.DAL.DAO
{
    public class UserDAO : IUserDAO
    {
        protected readonly PortalDBContext _portalDBContext;

        public UserDAO(PortalDBContext portalDBContext)
        {
            _portalDBContext = portalDBContext;
        }

        public virtual bool Exist(string userName)
        {
            return _portalDBContext.UserData
                .Where(x => x.UserName == userName)
                .Any();
        }

        public virtual bool RegisterUser(UserModel userModel)
        {
            if (_portalDBContext.UserData.Any(t => t.UserName == userModel.UserName))
            {
                return false;
            }

            UserDBModel user = new UserDBModel
            {
                Name = userModel.Name,
                UserName = userModel.UserName,
                Password = userModel.Password,
                Surname = userModel.Surname,
                Role = userModel.UserRight
            };

            _portalDBContext.UserData.Add(user);

            int changes = _portalDBContext.SaveChanges();
            return changes > 0;
        }

        public virtual bool ConfirmToken(string userName)
        {
            UserDBModel user = GetUser(userName);
            if (user == null)
            {
                return false;
            }

            user.Confirmed = true;

            _portalDBContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            int changes = _portalDBContext.SaveChanges();

            return changes > 0;
        }

        public virtual bool UpdatePassword(string userName, string password)
        {
            UserDBModel user = GetUser(userName);
            if (user != null)
            {
                user.Password = password;
                return true;
            }
            return false;
        }

        public virtual bool ValidatePassword(string userName, string pass)
        {
            if (_portalDBContext.UserData.Any(t => t.UserName == userName && t.Password == pass && t.Confirmed))
            {
                return true;
            }

            return false;
        }

        public virtual UserDBModel GetUser(string userName)
        {
            return _portalDBContext.UserData.FirstOrDefault(t => t.UserName == userName);
        }

        public virtual bool SaveSession(string session, DateTime expires)
        {
            _portalDBContext.Sessions.Add(new SessionDBModel
            {
                ExpirationDateTime = expires,
                SessionId = session
            });

            int changes = _portalDBContext.SaveChanges();
            return changes > 0;
        }

        public virtual bool ValidateSession(string session)
        {
            if (_portalDBContext.Sessions.Any(t => t.SessionId == session && t.ExpirationDateTime > DateTime.UtcNow))
            {
                return true;
            }

            return false;
        }

        public virtual List<UserDBModel> GetUsers()
        {
            return _portalDBContext.UserData.ToList();
        }

        public virtual List<UserDBModel> GetUsers(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                search = string.Empty;
            }

            return _portalDBContext.UserData
                .Where(x => x.UserName.Contains(search))
                .ToList();
        }
    }
}
