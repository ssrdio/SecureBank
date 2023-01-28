using SecureBank.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Interfaces
{
    public interface IStoreBL
    {
        Task<List<StoreItem>> GetStoreItems();
        Task<List<PurcahseHistoryItemResp>> GetPurchaseHistory(string userName);

        Task<bool> BuyProduct(BuyProductReq buyProductReq, string userName);
        Task<StoreItem> GetStoreItem(int id);
    }
}
