using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Interfaces;
using SecureBank.Models.Auth;

namespace SecureBank.Controllers
{
    public class AuthController : MvcBaseContoller
    {
        private readonly IUserDAO _userDAO;
        private readonly ITransactionDAO _transactionDAO;
        private readonly IEmailSender _emailSender;

        private readonly IAuthBL _authBL;

        public AuthController(
            IUserDAO userDAO,
            ITransactionDAO transactionDAO,
            IEmailSender emailSender,
            IAuthBL authBL)
        {
            _userDAO = userDAO;
            _emailSender = emailSender;
            _transactionDAO = transactionDAO;

            _authBL = authBL;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.ShowForgotPassword = !_authBL.IgnoreMails();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _authBL.Logout(returnUrl);

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/home/index");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.ConfirmMail = !_authBL.IgnoreMails();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmRegistration([FromQuery] string token)
        {
            bool ignoreMails = _authBL.IgnoreMails();
            if (ignoreMails)
            {
                return NotFound();
            }

            bool confirmRegistrationResult = await _authBL.ConfirmRegistration(token);
            if (!confirmRegistrationResult)
            {
                ViewBag.Status = "invalid";
            }
            else
            {
                ViewBag.Status = "ok";
            }

            return View();
        }

        [HttpGet]
        public IActionResult PasswordRecovery()
        {
            bool ignoreMails = _authBL.IgnoreMails();
            if (ignoreMails)
            {
                return NotFound();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Recover([FromQuery] string token)
        {
            bool ignoreMails = _authBL.IgnoreMails();
            if (ignoreMails)
            {
                return NotFound();
            }

            bool recoverPasswordTokenValid = await _authBL.RecoverPasswordValid(token);
            if (!recoverPasswordTokenValid)
            {
                ViewBag.Status = "invalid";
            }
            else
            {
                ViewBag.Status = "ok";
            }

            ViewBag.Token = token;

            return View();
        }

        [HttpGet]
        public IActionResult RecoverConfirm()
        {
            bool ignoreMails = _authBL.IgnoreMails();
            if (ignoreMails)
            {
                return NotFound();
            }

            return View();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public IActionResult GetTestUser()
        {
            UserModel userModel = _authBL.GetUser("tester@ssrd.io");
            return Ok(userModel);
        }
    }
   
}