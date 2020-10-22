using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Helpers;
using SecureBank.Ctf.Models;
using Microsoft.Extensions.Options;
using System.Linq;
using SecureBank.Interfaces;
using System.Threading.Tasks;

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
