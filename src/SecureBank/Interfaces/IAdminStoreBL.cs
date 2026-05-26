using SecureBank.Models;
using SecureBank.Models.Store;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IAdminStoreBL
    {
        string GetIndexViewName();

        Task<List<PurcahseHistoryItemResp>> GetAllPurchases();
        Task<DataTableResp<StoreItem>> GetStoreItems(HttpContext httpContext = null);

        Task<bool> CreateStoreItem(StoreItem storeItem);

        Task<StoreItem> GetStoreItem(int id);
        Task<StoreItem> EditStoreItem(int id, StoreItem storeItem);

        Task<bool> DeleteStoreItem(StoreItem storeItem);
    }
}
