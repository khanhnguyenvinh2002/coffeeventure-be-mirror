using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using coffeeventureAPI.Core.Base.Model;
using coffeeventureAPI.Service.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RoleEntity = coffeeventureAPI.Data.Role;
using UserEntity = coffeeventureAPI.Data.User;
using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace coffeeventureAPI.Controllers
{
    [Route("account")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IAccountService _accountService;

        public AccountController(IConfiguration configuration, IAccountService accountService)
        {
            _config = configuration;
            _accountService = accountService;
        }

        [Route("logout")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> Logout()
        {
            string idToken = Request.Headers["AccessToken"];
            var cookieOptions = new CookieOptions() { Expires = DateTime.Now.AddMonths(-1) };
            Response.Cookies.Append("AccessToken", string.Empty, cookieOptions);
            return await Task.FromResult(true);
        }
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<AccountChildDto> Login([FromBody] LoginRequestDto login)
        {
            return await _accountService.LoginChild(login);
            //IActionResult response = Unauthorized();

        }
        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<AccountChildDto> Register([FromBody] LoginRequestDto login)
        {
            return await _accountService.Register(login);
            //IActionResult response = Unauthorized();

        }
        public class LoginRequestDto
        {
            public string Email { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool RememberLogin { get; set; }
            public string ReturnUrl { get; set; }
        }
        public class AccountChildDto
        {
            public string AccessToken { get; set; }
            public bool CanAccess { get; set; }
            public AccountChildInfo UserInfo { get; set; }
            public string Redirect { get; set; }
        }
        public class AccountChildInfo : BaseModel
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Avatar { get; set; }
            public string Language { get; set; }
            public List<RoleEntity> Roles { get; set; }

            public AccountChildInfo(UserEntity user) : base(user)
            {
            }
            public AccountChildInfo()
            {
            }
        }
       
    }
}