using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAPI.DAL;
using StoreAPI.DAL.DBModels;
using StoreAPI.Models;
using NLog;

namespace StoreAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Store/[action]")]
    public class StoreController : Controller
    {
        private readonly StoreContext _storeContext;
        static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public StoreController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpGet]
        public ActionResult GetStoreItems()
        {
            _logger.Trace($"Returned list of store items");
            return new ObjectResult(_storeContext.StoreItems.ToList());
        }

        [HttpPost]
        public ActionResult CreateStoreItem([FromBody] StoreItemTable item)
        {
            _logger.Trace($"New store item added, id: {item.Id}");
            _storeContext.StoreItems.Add(item);
            _storeContext.SaveChanges();

            return new ObjectResult(item);
        }

        [HttpPost]
        public async Task<ActionResult> EditStoreItem([FromBody] StoreItemTable item)
        {
            _logger.Trace($"Checking if store item with id {item.Id} exists, so it can be edited.");
            var storeItem = await _storeContext.StoreItems
                 .FirstOrDefaultAsync(m => m.Id == item.Id);
            if (storeItem == null)
            {
                _logger.Trace($"Store item with id {item.Id} doesn't exist.");
                return null;
            }

            _logger.Trace($"Store item with id {item.Id} exists.");
            return new ObjectResult(storeItem);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEditItem([FromBody] StoreItemTable item)
        {
            var storeItem = await _storeContext.StoreItems.FindAsync(item.Id);
            storeItem.Name = item.Name;
            storeItem.Description = item.Description;
            storeItem.Price = item.Price;
            storeItem.Installments = item.Installments;
            _storeContext.StoreItems.Update(storeItem);
            _logger.Trace($"Store item with id {item.Id} edited.");
            _storeContext.SaveChanges();

            return new ObjectResult(storeItem);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteItem([FromBody] StoreItemTable item)
        {
            _logger.Trace($"Checking if store item with id {item.Id} exists, so it can be deleted.");
            var storeItem = await _storeContext.StoreItems
                .FirstOrDefaultAsync(m => m.Id == item.Id);
            if (storeItem == null)
            {
                _logger.Trace($"Store item with id {item.Id} doesn't exist.");
                return null;
            }

            _logger.Trace($"Store item with id {item.Id} exists.");
            return new ObjectResult(storeItem);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDeleteItem([FromBody] StoreItemTable item)
        {
            var storeItem = await _storeContext.StoreItems.FindAsync(item.Id);
            _storeContext.StoreItems.Remove(storeItem);
            await _storeContext.SaveChangesAsync();

            _logger.Trace($"Store item with id {item.Id} deleted.");
            return new ObjectResult(storeItem);
        }

        [HttpPost]
        public ActionResult CheckoutBasket([FromBody] List<PurcahseItemReq> purchases)
        {
            foreach (var purchase in purchases)
            {
                var storeItem = _storeContext.StoreItems
                    .FirstOrDefault(t => t.Id == purchase.StoreItemId);

                if (storeItem == null)
                {
                    return BadRequest();
                }
                _logger.Trace($"User {purchases[0].Username} made a purchase {purchase.StoreItemId}.");
                _storeContext.Purchases.Add(new PurchaseItemTable
                {
                    DateTimePurchased = DateTime.Now,
                    Username = purchase.Username,
                    Description = storeItem.Description,
                    Quantity = purchase.Quantity,
                    StoreItemId = purchase.StoreItemId,
                    Name = storeItem.Name,
                    Price = purchase.Price
                });
            }
            _storeContext.SaveChanges();
            return Ok(new EmptyResp());
        }

        [HttpGet]
        public ActionResult GetAllPurchases([FromQuery] string user)
        {
            var purchaseItems = _storeContext.Purchases
                .Where(purchase => purchase.Username == user)
                .OrderBy(purchase => purchase.DateTimePurchased)
                .ToList();

            List<PurcahseHistoryItemResp> purchases = new List<PurcahseHistoryItemResp>();
            foreach (PurchaseItemTable purchaseItem in purchaseItems)
            {
                purchases.Add(new PurcahseHistoryItemResp
                {
                    Description = purchaseItem.Description,
                    Price = purchaseItem.Price,
                    PurchaseTime = purchaseItem.DateTimePurchased,
                    Quantity = purchaseItem.Quantity,
                    Name = purchaseItem.Name
                });
            }

            _logger.Trace($"Returned list of purchases for user {user}.");
            return Ok(purchases);
        }

        [HttpGet]
        public ActionResult AdminGetAllPurchases()
        {
            var purchaseItems = _storeContext.Purchases
                .OrderBy(purchase => purchase.DateTimePurchased)
                .ToList();

            List<PurcahseHistoryItemResp> purchases = new List<PurcahseHistoryItemResp>();
            foreach (PurchaseItemTable purchaseItem in purchaseItems)
            {
                purchases.Add(new PurcahseHistoryItemResp
                {
                    Description = purchaseItem.Description,
                    Price = purchaseItem.Price,
                    PurchaseTime = purchaseItem.DateTimePurchased,
                    Quantity = purchaseItem.Quantity,
                    UserName = purchaseItem.Username,
                    Name = purchaseItem.Name
                });
            }
            return Ok(purchases);
        }

        [HttpPost]
        public int GetPurchaseId([FromBody] User user)
        {
            var purchaseItems = _storeContext.Purchases
                .Where(purchase => purchase.Username == user.Username)
                .OrderBy(purchase => purchase.StoreItemId)
                .ToList();

            if (purchaseItems.Count == 0)
            {
                _logger.Trace($"Returned first purchaseId (0) for user {user.Username}.");
                return 0;
            }

            _logger.Trace($"Returned next purchaseId ({purchaseItems[^1].StoreItemId + 1}) "
                + "for user {user.Username}.");
            return purchaseItems[^1].StoreItemId + 1;
        }
    }
}
