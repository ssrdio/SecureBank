using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Helpers;

namespace SecureBank.Controllers
{
    [Authenticate]
    public class HomeController : MvcBaseContoller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
