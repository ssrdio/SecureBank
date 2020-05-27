using SecureBank.Helpers.Authorization.Attributes;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;
using SecureBank.Models.PortalSearch;

namespace SecureBank.Controllers.Api
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Api)]
    public class PortalSearchController : ApiBaseController
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

            return Ok(portalSearchModel);
        }
    }
}