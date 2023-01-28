using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models.Store;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureBank.Controllers.Api
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Api)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class StoreController : ApiBaseController
    {
        private readonly IStoreBL _storeBL;

        public StoreController(IStoreBL storeBL)
        {
            _storeBL = storeBL;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<StoreItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStoreItems()
        {
            List<StoreItem> storeItems = await _storeBL.GetStoreItems();

            return Ok(storeItems);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PurcahseHistoryItemResp>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistory([FromQuery] string username = null)
        {
            if(string.IsNullOrEmpty(username))
            {
                username = HttpContext.GetUserName();
            }

            List<PurcahseHistoryItemResp> history = await _storeBL.GetPurchaseHistory(username);

            return Ok(history);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuyProduct([FromBody] BuyProductReq buyProductReq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            bool result = await _storeBL.BuyProduct(buyProductReq, HttpContext.GetUserName());
            if (!result)
            {
                return BadRequest();
            }

            return Ok(new EmptyResult());
        }
    }
}
