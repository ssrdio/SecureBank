using SecureBank.Models;
using Microsoft.Extensions.Options;
using SecureBank.Helpers;
using SecureBank.Models.Store;
using SecureBank.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SecureBank.Services
{
    public class StoreAPICalls
    {
        private OutCalls _outCalls;

        public StoreAPICalls(IOptions<AppSettings> appSettings)
        {
            _outCalls = new OutCalls(appSettings.Value.StoreEndpoint.ApiUrl);
        }

        public async Task<List<StoreItem>> GetStoreItemsAsync()
        {
            string url =  "GetStoreItems";

            List<StoreItem> storeItems = await _outCalls.GetData<List<StoreItem>>(url);
            if(storeItems == null)
            {
                return new List<StoreItem>();
            }

            return storeItems;
        }

        public Task<StoreItem> EditStoreItemAsync(StoreItem item)
        {
            string url =  "EditStoreItem";

            return _outCalls.PostData<StoreItem>(url, item);
        }

        public Task<StoreItem> ConfirmEditStoreItemAsync(StoreItem item)
        {
            string url = "ConfirmEditItem";
            return _outCalls.PostData<StoreItem>(url, item);
        }

        public Task<StoreItem> CreateStoreItemAsync(StoreItem item)
        {
            string url = "CreateStoreItem";

            return _outCalls.PostData<StoreItem>(url, item);
        }

        public Task<StoreItem> DeleteStoreItemAsync(StoreItem item)
        {
            string url =  "DeleteItem";

            return _outCalls.PostData<StoreItem>(url, item);
        }

        public Task<StoreItem> ConfirmDeleteStoreItemAsync(StoreItem item)
        {
            string url =  "ConfirmDeleteItem";
            return _outCalls.PostData<StoreItem>(url, item);
        }

        public Task<EmptyResult> CheckoutBasketAsync(List<PurchaseItemReq> purchases)
        {
            string url = "CheckoutBasket";
            return _outCalls.PostData<EmptyResult>(url, purchases);
        }

        public async Task<List<PurcahseHistoryItemResp>> GetAllPurchasesAsync(UserInfoReq user)
        {
            string url =  $"GetAllPurchases?user={user.UserName}";

            List<PurcahseHistoryItemResp> purchases = await _outCalls.GetData<List<PurcahseHistoryItemResp>>(url);
            if (purchases == null)
            {
                return new List<PurcahseHistoryItemResp>();
            }

            return purchases;
        }

        public async Task<List<PurcahseHistoryItemResp>> AdminGetAllPurchasesAsync()
        {
            string url = "AdminGetAllPurchases";

            List<PurcahseHistoryItemResp> purchaseHistory = await _outCalls.GetData<List<PurcahseHistoryItemResp>>(url);
            if (purchaseHistory == null)
            {
                return new List<PurcahseHistoryItemResp>();
            }

            return purchaseHistory;
        }

        public Task<int> GetPurchaseIdAsync(UserInfoReq user)
        {
            string url = "GetPurchaseId";

            return _outCalls.PostData<int>(url, user);
        }
    }
}
