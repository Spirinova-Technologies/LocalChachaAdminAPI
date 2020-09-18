using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoginController> logger;

        public LoginController(IUserService userService, ITokenService tokenService, ILogger<LoginController> logger)
        {
            this.userService = userService;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        [HttpPost("authenticate")]
        public ActionResult Authenticate([FromBody] LoginViewModel loginModel)
        {
            try
            {
                logger.LogInformation("Called login.");
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