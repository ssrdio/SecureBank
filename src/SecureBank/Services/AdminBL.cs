using SecureBank.DAL.DAO;
using Microsoft.Extensions.Options;
using NLog;
using SecureBank.DAL.DBModels;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using SecureBank.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Services
{
    public class AdminBL : IAdminBL
    {
        /// <summary>
        /// for checking if a transaction is an in-app purchase or not
        /// </summary>
        private const string BANK_WEB_APP = "BankWebApp";

        private readonly ITransactionDAO _transactionDao;
        private readonly IUserDAO _userDAO;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public AdminBL(ITransactionDAO transactionDao, IUserDAO userDAO)
        {
            _transactionDao = transactionDao;
            _userDAO = userDAO;
        }

        public virtual string GetIndexViewName()
        {
            return "Index";
        }

        public virtual DataTableResp<TransactionResp> GetTransactions()
        {
            List<TransactionResp> transactionRespList = new List<TransactionResp>();
            List<TransactionResp> transactionDaoList = _transactionDao.GetTransactions();
            foreach (TransactionResp transactionResp in transactionDaoList)
            {
                if (transactionResp.ReceiverId == BANK_WEB_APP)
                {
                    UserDBModel userDBModel = _userDAO.GetUser(transactionResp.SenderId);
                    if (userDBModel != null)
                    {
                        transactionRespList.Add(new TransactionResp
                        {
                            Id = transactionResp.Id,
                            SenderId = transactionResp.SenderId,
                            ReceiverId = transactionResp.ReceiverId,
                            DateTime = transactionResp.DateTime,
                            Reason = transactionResp.Reason,
                            Amount = transactionResp.Amount,
                            Reference = transactionResp.Reference,
                            SenderName = userDBModel.Name,
                            SenderSurname = userDBModel.Surname,
                            ReceiverName = BANK_WEB_APP,
                            ReceiverSurname = "Store"
                        });
                    }
                    else
                    {
                        _logger.Trace($"Unknown user with senderId {transactionResp.SenderId}");
                        transactionRespList.Add(new TransactionResp
                        {
                            Id = transactionResp.Id,
                            SenderId = transactionResp.SenderId,
                            ReceiverId = transactionResp.ReceiverId,
                            DateTime = transactionResp.DateTime,
                            Reason = transactionResp.Reason,
                            Amount = transactionResp.Amount,
                            Reference = transactionResp.Reference,
                            SenderName = "unknown",
                            SenderSurname = "user",
                            ReceiverName = "BankWebApp",
                            ReceiverSurname = "Store"
                        });
                    }
                }
                else
                {
                    UserDBModel senderModel = _userDAO.GetUser(transactionResp.SenderId);
                    UserDBModel receiverModel = _userDAO.GetUser(transactionResp.SenderId);
                    if (senderModel != null && receiverModel != null)
                    {
                        transactionRespList.Add(new TransactionResp
                        {
                            Id = transactionResp.Id,
                            SenderId = transactionResp.SenderId,
                            ReceiverId = transactionResp.ReceiverId,
                            DateTime = transactionResp.DateTime,
                            Reason = transactionResp.Reason,
                            Amount = transactionResp.Amount,
                            Reference = transactionResp.Reference,
                            SenderName = senderModel.Name,
                            SenderSurname = senderModel.Surname,
                            ReceiverName = receiverModel.Name,
                            ReceiverSurname = receiverModel.Surname
                        });
                    }
                    else if (senderModel == null && receiverModel == null)
                    {
                        continue;
                    }
                    else if (senderModel == null)
                    {
                        _logger.Trace($"Unknown user with senderId {transactionResp.SenderId}");
                        transactionRespList.Add(new TransactionResp
                        {
                            Id = transactionResp.Id,
                            SenderId = transactionResp.SenderId,
                            ReceiverId = transactionResp.ReceiverId,
                            DateTime = transactionResp.DateTime,
                            Reason = transactionResp.Reason,
                            Amount = transactionResp.Amount,
                            Reference = transactionResp.Reference,
                            SenderName = "unknown",
                            SenderSurname = "user",
                            ReceiverName = receiverModel.Name,
                            ReceiverSurname = receiverModel.Surname
                        });
                    }
                    else
                    {
                        _logger.Trace($"Unknown user with receiverId {transactionResp.ReceiverId}");
                        transactionRespList.Add(new TransactionResp
                        {
                            Id = transactionResp.Id,
                            SenderId = transactionResp.SenderId,
                            ReceiverId = transactionResp.ReceiverId,
                            DateTime = transactionResp.DateTime,
                            Reason = transactionResp.Reason,
                            Amount = transactionResp.Amount,
                            Reference = transactionResp.Reference,
                            SenderName = receiverModel.Name,
                            SenderSurname = receiverModel.Surname,
                            ReceiverName = "unknown",
                            ReceiverSurname = "user"
                        });
                    }
                }
            }

            return new DataTableResp<TransactionResp>(
                recordsTotal: transactionRespList.Count,
                recordsFiltered: transactionRespList.Count,
                data: transactionRespList);
        }

        public virtual DataTableResp<AdminUserInfoResp> GetUsers()
        {
            List<AdminUserInfoResp> users = _userDAO.GetUsers()
                .Select(t => new AdminUserInfoResp
                {
                    Name = t.Name,
                    Surname = t.Surname,
                    Username = t.UserName,
                    Role = t.Role
                })
                .ToList();

            return new DataTableResp<AdminUserInfoResp>(
                recordsTotal: users.Count,
                recordsFiltered: users.Count,
                data: users);
        }
    }
}
