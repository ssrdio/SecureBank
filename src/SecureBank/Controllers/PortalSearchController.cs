using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;
using SecureBank.Models.PortalSearch;

namespace SecureBank.Controllers
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Mvc)]
    public class PortalSearchController : MvcBaseContoller
    {
        private readonly IPortalSearchBL _portalSearchBL;

        public PortalSearchController(IPortalSearchBL portalSearchBL)
        {
            _portalSearchBL = portalSearchBL;
        }

        [HttpGet]
        public IActionResult Index(string searchString)
        {
            PortalSearchModel portalSearchModel = _portalSearchBL.Search(searchString);

            return View(portalSearchModel);
        }
    }
}