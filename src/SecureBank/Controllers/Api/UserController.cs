using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;
using SecureBank.Models.User;

namespace SecureBank.Controllers.Api
{
    [AuthorizeMissing(AuthorizeAttributeTypes.Api)]
    public class UserController : ApiBaseController
    {
        private readonly IUserBL _userBL;

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("image/jpg")]
        public IActionResult ProfileImage([FromQuery] string user)
        {
            byte[] file = _userBL.GetProfileImage(user);

            return File(file, "image/jpg");
        }

        [HttpGet]
        [ProducesResponseType(typeof(AccountBalanceResp), StatusCodes.Status200OK)]
        public IActionResult GetAvailableFunds([FromQuery] string user)
        {
            AccountBalanceResp accountBalance = _userBL.GetAmount(user);

            return Ok(accountBalance);
        }
    }
}
