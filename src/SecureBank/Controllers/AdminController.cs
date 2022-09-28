using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Interfaces;

namespace SecureBank.Controllers
{
    [AuthorizeAdmin(AuthorizeAttributeTypes.Mvc)]
    public class AdminController : MvcBaseContoller
    {
        private readonly IAdminBL _adminBL;

        public AdminController(IAdminBL adminBL)
        {
            _adminBL = adminBL;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string viewName = _adminBL.GetIndexViewName();

            return View(viewName);
        }
    }
}