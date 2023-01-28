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
using System.Net.Http;

namespace SecureBank.Services
{
    public class UserBL : IUserBL
    {
        protected readonly ITransactionDAO _transactionDAO;
        protected readonly IUserDAO _userDAO;

        protected readonly IWebHostEnvironment _webHostEnvironment;

        protected readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        protected const string BASE_FOLDER = "SecureFiles/Images/";

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
            double balance = _transactionDAO.GetAccountBalance(userName);

            AccountBalanceResp accountBalance = new AccountBalanceResp(
                balance: Math.Round(balance, 3));

            return accountBalance;
        }

        public virtual async Task<bool> SetProfileImageUrl(string username, string url)
        {
            // User can set the profile picture as any URL, intended to get images from
            // another server, but can actually do get requests in name of server instead.
            // CWE-918: Server-Side Request Forgery (SSRF)
            try
            {
                // Response from get request.
                byte[] responseBytes = await new HttpClient().GetByteArrayAsync(url);

                // Setting the path of file.
                string contentRootPath = _webHostEnvironment.ContentRootPath;
                string path = System.IO.Path.Combine(contentRootPath, BASE_FOLDER);
                string userPath = System.IO.Path.Combine(path, username);

                await System.IO.File.WriteAllBytesAsync(userPath, responseBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual byte[] GetProfileImage(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Array.Empty<byte>();
            }

            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = System.IO.Path.Combine(contentRootPath, BASE_FOLDER);
            string userPath = System.IO.Path.Combine(path, userName);

            byte[] data;

            if (System.IO.File.Exists(userPath))
            {
                data = System.IO.File.ReadAllBytes(userPath);
            }
            else
            {
                try
                {
                    if (userName.Contains("@"))
                    {
                        string[] listFiles = System.IO.Directory.GetFiles(path);
                        data = System.IO.File.ReadAllBytes(listFiles[^1]);
                    }
                    else
                    {
                        data = System.IO.File.ReadAllBytes(userPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to get user image");
                    return Array.Empty<byte>();
                }
            }

            return data;
        }
    }
}
