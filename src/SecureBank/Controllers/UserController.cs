using Microsoft.AspNetCore.Mvc;
using SecureBank.DAL.DBModels;
using SecureBank.Helpers;
using SecureBank.Helpers.Authorization.Attributes;
using SecureBank.Interfaces;
using SecureBank.Models;

namespace SecureBank.Controllers
{
    [AuthorizeNormal(AuthorizeAttributeTypes.Mvc)]
    public class UserController : MvcBaseContoller
    {
        private readonly IUserDAO _userDAO;
        private readonly IUserBL _userBL;

        public UserController(IUserDAO userDAO, IUserBL userBL)
        {
            _userDAO = userDAO;
            _userBL = userBL;
        }

        public IActionResult ProfilePage()
        {
            string userName = HttpContext.GetUserName();
            UserDBModel user = _userDAO.GetUser(userName);

            var model = new ProfilePageViewModel
            {
                Username = user?.UserName ?? userName,
                Name = user?.Name,
                Surname = user?.Surname,
                ImageUrl = $"/api/User/ProfileImage?user={userName}",
                Balance = _userBL.GetAmount(userName, HttpContext)?.Balance ?? 0
            };

            return View(model);
        }
    }
}



