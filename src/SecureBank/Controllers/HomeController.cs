using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Helpers;
using SecureBank.Ctf.Models;
using Microsoft.Extensions.Options;
using System.Linq;

namespace SecureBank.Controllers
{
    [Authenticate]
    public class HomeController : MvcBaseContoller
    {
        private readonly CtfOptions _ctfOptions;
        public HomeController(IOptions<CtfOptions> options)
        {
            _ctfOptions = options.Value;
        }

        public IActionResult Index()
        {
            if (_ctfOptions.CtfChallanges != null && _ctfOptions.CtfChallanges.Any())
            {
                ViewBag.HiddenComment = _ctfOptions.CtfChallanges.Where(x => x.Type == CtfChallengeTypes.HiddenComment)
                    .Single().Flag;
                ViewBag.Base = _ctfOptions.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.Base2048Content)
                    .Single().Flag;
            }
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
