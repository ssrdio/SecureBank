using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using SecureBank.Models.User;

namespace SecureBank.Controllers.Api
{
    [AuthorizeAdmin(AuthorizeAttributeTypes.Api)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class AdminController : ApiBaseController
    {
        private readonly IAdminBL _adminBL;

        public AdminController(IAdminBL adminBL)
        {
            _adminBL = adminBL;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResp<TransactionResp>), StatusCodes.Status200OK)]
        public IActionResult Transactions()
        {
            DataTableResp<TransactionResp> transactions = _adminBL.GetTransactions();

            return Ok(transactions);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResp<AdminUserInfoResp>), StatusCodes.Status200OK)]
        public IActionResult GetAllUsers()
        {
            DataTableResp<AdminUserInfoResp> users = _adminBL.GetUsers();

            return Ok(users);
        }
    }
}