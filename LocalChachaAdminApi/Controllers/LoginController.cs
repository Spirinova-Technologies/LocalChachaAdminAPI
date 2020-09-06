using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LocalChachaAdminApi.Controllers
{
    [Route("api/login")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;

        public LoginController(IUserService userService, ITokenService tokenService)
        {
            this.userService = userService;
            this.tokenService = tokenService;
        }

        [HttpPost("authenticate")]
        public ActionResult Authenticate([FromBody] LoginViewModel loginModel)

        {
            try
            {
                //TODO: remove try catch and add globalexception filter
                var loginRequest = new LoginModel
                {
                    Password = loginModel.Password,
                    Username = loginModel.Username
                };

                var user = userService.Authenticate(loginRequest);

                if (user == null)
                {
                    return NotAcceptable("Invalid username or password");
                }

                var token = tokenService.GenerateToken(user.Id);

                return Ok(new { authToken = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}