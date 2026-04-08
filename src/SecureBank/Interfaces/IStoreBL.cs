using SecureBank.Models.Store;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IStoreBL
    {
        Task<List<StoreItem>> GetStoreItems();
        Task<List<PurcahseHistoryItemResp>> GetPurchaseHistory(string userName, HttpContext httpContext = null);

        Task<bool> BuyProduct(BuyProductReq buyProductReq, string userName, HttpContext httpContext = null);
        Task<StoreItem> GetStoreItem(int id);
    }
}
