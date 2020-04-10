using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Controllers.Api
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Api)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class SearchController : ApiBaseController
    {
        private readonly IUserBL _userBL;

        public SearchController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public IActionResult FindUser([FromQuery] string term)
        {
            List<string> users = _userBL.FindUsers(term);

            return Ok(users);
        }
    }
}
