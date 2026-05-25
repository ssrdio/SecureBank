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
            if (!_ctfSettings.Enabled || string.IsNullOrEmpty(_ctfSettings.GeneratedFlag))
            {
                return Ok("Flag is disabled");
            }

            return Ok(_ctfSettings.GeneratedFlag);
        }
    }
}
