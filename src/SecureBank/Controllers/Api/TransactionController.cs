using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Models.Transaction;
using System.Collections.Generic;

namespace SecureBank.Controllers.Api
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Api)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class TransactionController : ApiBaseController
    {
        private readonly ITransactionBL _transactionBL;

        public TransactionController(ITransactionBL transactionBL)
        {
            _transactionBL = transactionBL;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TransactionDBModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get([FromRoute] int id)
        {
            TransactionDBModel transaction = _transactionBL.Details(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResp<TransactionResp>), StatusCodes.Status200OK)]
        public IActionResult GetTransactions(
            [FromQuery] int start,
            [FromQuery] int length,
            [FromQuery(Name = "search[value]")] string search)
        {
            DataTableResp<TransactionResp> dataTableResp = _transactionBL.GetTransactions(
                HttpContext.GetUserName(), search, start, length);

            return Ok(dataTableResp);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TransactionResp>), StatusCodes.Status200OK)]
        public IActionResult GetTransactionHistory([FromQuery] string userName)
        {
            List<TransactionsByDayResp> transactions = _transactionBL.GetTransactionsByDay(userName);

            return Ok(transactions);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] TransactionDBModel transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool createResult = _transactionBL.Create(transaction);
            if (!createResult)
            {
                return BadRequest();
            }

            return Ok(new EmptyResult());
        }
    }
}
