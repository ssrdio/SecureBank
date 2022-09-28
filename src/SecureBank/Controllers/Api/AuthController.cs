using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureBank.Filters;
using SecureBank.Interfaces;
using SecureBank.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Controllers.Api
{
    public class AuthController : ApiBaseController
    {
        private readonly IAuthBL _authBL;

        public AuthController(IAuthBL authBL)
        {
            _authBL = authBL;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel userModel = await _authBL.Login(loginModel);
            if (userModel == null)
            {
                return BadRequest();
            }

            return Ok(loginModel);
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult LoginAdmin()
        {
            return _authBL.LoginAdmin();
        }

        [HttpGet]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await _authBL.Logout(null);

            return Ok(new EmptyResult());
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            bool registrationResult = await _authBL.Register(registrationModel);
            if (!registrationResult)
            {
                return BadRequest();
            }

            return Ok(new EmptyResult());
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[HiddenPage]
        //[HttpPost]
        //public void RegisterAdmin([FromBody] UserModel userModel)
        //{
        //}

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PasswordRecovery([FromBody] UserModel userModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            bool passwordRecovery = await _authBL.PasswordRecovery(userModel);
            if(!passwordRecovery)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Recover([FromBody] UserModel passwordRecoveryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            bool recoverPasswordResult = await _authBL.RecoverPassword(passwordRecoveryModel);
            if (!recoverPasswordResult)
            {
                return BadRequest();
            }

            return Ok(new EmptyResult());
        }
    }
}
