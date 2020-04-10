using SecureBank.DAL.DAO;
using Microsoft.AspNetCore.Hosting;
using NLog;
using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using SecureBank.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Services
{
    public class UserBL : IUserBL
    {
        private readonly ITransactionDAO _transactionDAO;
        private readonly IUserDAO _userDAO;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public UserBL(ITransactionDAO transactionDAO, IUserDAO userDAO, IWebHostEnvironment webHostEnvironment)
        {
            _transactionDAO = transactionDAO;
            _userDAO = userDAO;

            _webHostEnvironment = webHostEnvironment;
        }

        public virtual List<string> FindUsers(string search)
        {
            List<UserDBModel> users = _userDAO.GetUsers(search);

            return users
                .Select(x => x.UserName)
                .ToList();
        }

        public virtual AccountBalanceResp GetAmount(string userName)
        {
            double balance = _transactionDAO.GetAccountbalance(userName);

            AccountBalanceResp accountBalance = new AccountBalanceResp(
                balance: Math.Round(balance, 3));

            return accountBalance;
        }

        public virtual byte[] GetProfileImage(string userName)
        {
            if(string.IsNullOrEmpty(userName))
            {
                return Array.Empty<byte>();
            }

            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = System.IO.Path.Combine(contentRootPath, "SecureFiles/Images/");
            byte[] data = Array.Empty<byte>();

            string userPath = System.IO.Path.Combine(path, userName);

            if (System.IO.File.Exists(userPath))
            {
                data = System.IO.File.ReadAllBytes(userPath);
            }
            else
            {
                if (userName.Contains("@"))
                {
                    //TO DO: that can be an flag

                    string[] listFiles = System.IO.Directory.GetFiles(path);
                    data = System.IO.File.ReadAllBytes(listFiles[listFiles.Length - 1]);
                }
                else
                {
                    data = System.IO.File.ReadAllBytes(userPath);
                }
            }

            return data;
        }
    }
}
