using SecureBank.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Interfaces;
using SecureBank.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Controllers.Api
{
    [AuthorizeAdmin(AuthorizeAttributeTypes.Api)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class AdminStoreController : ApiBaseController
    {
        private readonly IAdminStoreBL _adminStoreBL;

        public AdminStoreController(IAdminStoreBL adminStoreBL)
        {
            _adminStoreBL = adminStoreBL;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<List<PurcahseHistoryItemResp>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPurchases()
        {
            List<PurcahseHistoryItemResp> resp = await _adminStoreBL.GetAllPurchases();
            if (resp == null)
            {
                return BadRequest();
            }

            return Ok(resp);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<StoreItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStoreItems()
        {
            DataTableResp<StoreItem> dataTableResp = await _adminStoreBL.GetStoreItems();
            if (dataTableResp == null)
            {
                return BadRequest();
            }

            return Ok(dataTableResp);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody]StoreItem storeItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = await _adminStoreBL.CreateStoreItem(storeItem);
            if (!result)
            {
                return BadRequest();
            }

            return Ok(new EmptyResult());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StoreItem), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            StoreItem storeItem = await _adminStoreBL.GetStoreItem(id);
            if (storeItem == null)
            {
                return BadRequest();
            }

            return Ok(storeItem);
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(StoreItem), StatusCodes.Status200OK)]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] StoreItem storeItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            StoreItem editedStoreItem = await _adminStoreBL.EditStoreItem(id, storeItem);
            if (editedStoreItem == null)
            {
                return BadRequest();
            }

            return Ok(editedStoreItem);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteStoreItem([FromBody] StoreItem storeItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool deleteResult = await _adminStoreBL.DeleteStoreItem(storeItem);
            if (!deleteResult)
            {
                return BadRequest();
            }

            return Ok(new EmptyResult());
        }
    }
}
