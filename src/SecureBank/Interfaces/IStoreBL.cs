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
        Task<List<PurcahseHistoryItemResp>> GetPurcahseHistory(string userName);

        Task<bool> BuyProduct(BuyProductReq buyProductReq, string userName);
    }
}
