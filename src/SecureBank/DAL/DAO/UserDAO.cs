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

        public virtual bool UpdatePasswordToken(string userName, string token)
        {
            var user = _portalDBContext.UserData
                .FirstOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                return false;
            }
            user.RecoveryGuid = token;
            _portalDBContext.SaveChanges();
            return true;
        }

        public virtual bool PasswordTokenExists(string token)
        {
            return _portalDBContext.UserData
                .Any(x => x.RecoveryGuid == token);
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
            if (user == null)
            {
                return false;
            }

            user.Password = password;

            _portalDBContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            int changes = _portalDBContext.SaveChanges();

            return changes > 0;
        }

        public virtual bool ValidatePassword(string userName, string pass, bool requireConfirmed)
        {
            IQueryable<UserDBModel> query = _portalDBContext.UserData
                .Where(x => x.UserName == userName)
                .Where(x => x.Password == pass);

            if(requireConfirmed)
            {
                query = query.Where(x => x.Confirmed);
            }

            return query.Any();
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
