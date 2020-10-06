using SecureBank.DAL.DAO;
using SecureBank.Models;
using SecureBank.Interfaces;
using SecureBank.Models.Store;
using SecureBank.Models.Transaction;
using SecureBank.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SecureBank.Services
{
    public class StoreBL : IStoreBL
    {
        protected const string TRANSACTION_REASON = "product";
        protected const string TRANSACTION_RECIVER_ID = "store";

        protected readonly ITransactionDAO _transactionDAO;
        protected readonly StoreAPICalls _storeAPICalls;

        public StoreBL(ITransactionDAO transactionDAO, StoreAPICalls storeAPICalls)
        {
            _transactionDAO = transactionDAO;
            _storeAPICalls = storeAPICalls;
        }

        public virtual async Task<bool> BuyProduct(BuyProductReq buyProductReq, string userName)
        {
            bool payResult = await Pay(buyProductReq, userName);
            if(!payResult)
            {
                return false;
            }

            // buy product id gets validated int the overwritten Pay method
            List<PurchaseItemReq> purchaseItems = new List<PurchaseItemReq>
            {
                new PurchaseItemReq
                {
                    Price = buyProductReq.Price,
                    Username = userName,
                    StoreItemId = buyProductReq.Id,
                    Quantity = buyProductReq.Quantity,
                }
            };

            EmptyResult checkoutResult = await _storeAPICalls.CheckoutBasketAsync(purchaseItems);
            if (checkoutResult == null)
            {
                return false;
            }

            return true;
        }

        protected virtual Task<bool> Pay(BuyProductReq buyProductReq, string userName)
        {
            double accountBalance = _transactionDAO.GetAccountbalance(userName);

            double ammountToPay = buyProductReq.Price * buyProductReq.Quantity;
            if (accountBalance < ammountToPay)
            {
                return Task.FromResult(false);
            }

            DepositRequest depositRequest = new DepositRequest
            {
                Amount = buyProductReq.Price * buyProductReq.Quantity,
                Reason = TRANSACTION_REASON,
                ReceiverId = TRANSACTION_RECIVER_ID,
                SenderId = userName
            };

            bool payResult = _transactionDAO.Pay(depositRequest);

            return Task.FromResult(payResult);
        }

        public virtual async Task<List<PurcahseHistoryItemResp>> GetPurcahseHistory(string userName)
        {
            UserInfoReq userInfoReq = new UserInfoReq
            {
                UserName = userName,
            };

            List<PurcahseHistoryItemResp> purchasedItems = await _storeAPICalls.GetAllPurchasesAsync(userInfoReq);

            return purchasedItems;
        }

        public virtual async Task<List<StoreItem>> GetStoreItems()
        {
            List<StoreItem> storeItems = await _storeAPICalls.GetStoreItemsAsync();

            return storeItems;
        }
    }
}
