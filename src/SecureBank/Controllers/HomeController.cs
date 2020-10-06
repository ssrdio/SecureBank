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
        private readonly IHomeBL _homeService;

        public HomeController(IHomeBL homeService)
        {
            _homeService = homeService;
        }

        public IActionResult Index()
        {
            _homeService.Index();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public Task<IActionResult> DownloadAndroidApp()
        {
            return _homeService.DownloadAndroidApp();
        }
    }
}
