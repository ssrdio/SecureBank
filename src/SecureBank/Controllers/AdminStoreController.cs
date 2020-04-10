using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Interfaces;
using SecureBank.Models.Store;

namespace SecureBank.Controllers
{
    [AuthorizeAdmin(AuthorizeAttributeTypes.Mvc)]
    public class AdminStoreController : MvcBaseContoller
    {
        private readonly IAdminStoreBL _adminStoreBL;

        public AdminStoreController(IAdminStoreBL adminStoreBL)
        {
            _adminStoreBL = adminStoreBL;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string viewName = _adminStoreBL.GetIndexViewName();

            return View(viewName);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] StoreItem storeItem)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool createResult = await _adminStoreBL.CreateStoreItem(storeItem);

            return Redirect(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StoreItem storeItem = await _adminStoreBL.GetStoreItem(id.Value);
            if (storeItem == null)
            {
                return NotFound();
            }

            return View(storeItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] StoreItem storeItem)
        {
            if (id != storeItem.Id)
            {
                return NotFound();
            }

            StoreItem editStoreItem = await _adminStoreBL.EditStoreItem(id, storeItem);
            if (editStoreItem == null)
            {
                ModelState.AddModelError("_error", "Error");
            }

            return View(editStoreItem);
        }
    }
}