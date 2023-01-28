using System.Collections.Generic;
using System.Threading.Tasks;
using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models.Store;

namespace SecureBank.Controllers
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Mvc)]
    public class StoreController : MvcBaseContoller
    {
        private readonly IStoreBL _storeBL;

        public StoreController(IStoreBL storeBL)
        {
            _storeBL = storeBL;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<StoreItem> storeItems = await _storeBL.GetStoreItems();

            return View(storeItems);
        }

        [HttpGet]
        public async Task<IActionResult> ItemDetails([FromQuery] int itemId)
        {
            StoreItem storeItems = await _storeBL.GetStoreItem(itemId);

            return View(storeItems);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            List<PurcahseHistoryItemResp> purchasedItems =
                await _storeBL.GetPurchaseHistory(HttpContext.GetUserName());

            return View(purchasedItems);
        }
    }
}