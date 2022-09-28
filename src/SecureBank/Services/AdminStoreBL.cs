using Newtonsoft.Json;
using NLog;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Services
{
    public class AdminStoreBL : IAdminStoreBL
    {
        private readonly StoreAPICalls _storeAPICalls;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public AdminStoreBL(StoreAPICalls storeAPICalls)
        {
            _storeAPICalls = storeAPICalls;
        }

        public virtual Task<List<PurcahseHistoryItemResp>> GetAllPurchases()
        {
            return _storeAPICalls.AdminGetAllPurchasesAsync();
        }

        public virtual async Task<DataTableResp<StoreItem>> GetStoreItems()
        {
            List<StoreItem> storeItems = await _storeAPICalls.GetStoreItemsAsync();

            DataTableResp<StoreItem> dataTableResp = new DataTableResp<StoreItem>(
                recordsTotal: storeItems.Count,
                recordsFiltered: storeItems.Count,
                data: storeItems);
            return dataTableResp;
        }

        public virtual async Task<bool> CreateStoreItem(StoreItem storeItem)
        {
            StoreItem resp = await _storeAPICalls.CreateStoreItemAsync(storeItem);
            if(resp == null)
            {
                _logger.Error("Failed to create new store item.\n"
                    + JsonConvert.SerializeObject(storeItem) );
                return false;
            }

            return true;
        }

        public virtual async Task<StoreItem> EditStoreItem(int id, StoreItem storeItem)
        {
            StoreItem editStoreItem = await _storeAPICalls.EditStoreItemAsync(storeItem);
            if(editStoreItem == null)
            {
                return null;
            }

            storeItem.Installments = editStoreItem.Installments;

            editStoreItem = await _storeAPICalls.ConfirmEditStoreItemAsync(storeItem);

            return editStoreItem;

        }

        public virtual async Task<StoreItem> GetStoreItem(int id)
        {
            List<StoreItem> storeItems = await _storeAPICalls.GetStoreItemsAsync();
            if(storeItems == null)
            {
                return null;
            }

            StoreItem storeItem = storeItems
                .Where(x => x.Id == id)
                .SingleOrDefault();

            return storeItem;
        }

        public virtual async Task<bool> DeleteStoreItem(StoreItem storeItem)
        {
            StoreItem deleteStoreItem = await _storeAPICalls.DeleteStoreItemAsync(storeItem);
            if(deleteStoreItem == null)
            {
                return false;
            }

            deleteStoreItem = await _storeAPICalls.ConfirmDeleteStoreItemAsync(deleteStoreItem);
            if(deleteStoreItem == null)
            {
                return false;
            }

            return true;
        }

        public virtual string GetIndexViewName()
        {
            return "Index";
        }
    }
}
