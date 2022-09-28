using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;
using SecureBank.Models.User;
using Microsoft.AspNetCore.StaticFiles;
using System.Threading.Tasks;
using System.Web;
using NLog;
using SecureBank.Models;

namespace SecureBank.Controllers.Api
{
    [AuthorizeMissing(AuthorizeAttributeTypes.Api)]
    public class UserController : ApiBaseController
    {
        private readonly IUserBL _userBL;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("text/plain")]
        public async Task<IActionResult> ProfileImage(NewImageModel image)
        {
            bool success = await _userBL.SetProfileImageUrl(image.Username, image.ImageUrl);
            if (success)
            {
                _logger.Info($"Setting new image for user {image.Username}");
                return Ok("New image set.");
            }
            else
            {
                return BadRequest("Failed to get resource.");
            }
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
