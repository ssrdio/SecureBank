using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models;

namespace StoreAPI.Controllers
{
    [Route("api/[controller]")]
    public class InternalController : Controller
    {
        private readonly CtfSettings _ctfSettings;

        public InternalController(CtfSettings ctfSettings)
        {
            _ctfSettings = ctfSettings;
        }

        [HttpGet("flag")]
        [Produces("text/plain")]
        public IActionResult Flag()
        {
            if (!_ctfSettings.Enabled || !_ctfSettings.SsrfChallenge || string.IsNullOrEmpty(_ctfSettings.GeneratedFlag))
            {
                return Ok("Flag disabled, good job though.");
            }

            return Ok(_ctfSettings.GeneratedFlag);
        }
    }
}
