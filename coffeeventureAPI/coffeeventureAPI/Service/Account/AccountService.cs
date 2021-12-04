using coffeeventureAPI.Core.Service;
using coffeeventureAPI.Core.Utilities;
using coffeeventureAPI.Model;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Repository.Account;
using coffeeventureAPI.Repository.Role;
using coffeeventureAPI.Repository.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;
using static coffeeventureAPI.Controllers.AccountController;
using UserModel = coffeeventureAPI.Data.User;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using RoleEntity = coffeeventureAPI.Data.Role;

namespace coffeeventureAPI.Service.Account
{

    /// <summary>
    /// Service class
    /// </summary>
    public class AccountService : BaseService, IAccountService
    {
        #region Private variables

        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IOperationService _operationService;
        private readonly ILogger<AccountService> _logger;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;


        #endregion Private variables


        public AccountService(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IUserService userService,
            IConfiguration configuration,
            ILogger<AccountService> logger,
            IOperationService operationService) : base()
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _operationService = operationService;
            _userService = userService;
            _config = configuration;
            _unitOfWork = unitOfWork;
            //_clientId = configuration["APIAuthentication:idaClientId"];
            //_clientSecret = configuration["APIAuthentication:idaClientSecret"];
            //_authority = configuration["APIAuthentication:idaAuthority"];
            //_redirect = configuration["APIAuthentication:idaRedirect"];
            //_discoverKey = configuration["APIAuthentication:idaADFSDiscoveryKey"];
            //_oauth = configuration["APIAuthentication:idaOauth"];
            _logger = logger;
        }

        public async Task<AccountChildDto> LoginChild(LoginRequestDto request)
        {
            var userInfo = new UserModel();
            if (!string.IsNullOrEmpty(request.UserName))
            {
                userInfo = (await _accountRepository.Select().ConfigureAwait(false))
                    .FirstOrDefault(x => x.UserName == request.UserName.Trim());
            }
            else if (!string.IsNullOrEmpty(request.Email))
            {
                userInfo = (await _accountRepository.Select().ConfigureAwait(false))
                    .FirstOrDefault(x => x.Email == request.Email.Trim());
            }
            var result = new AccountChildDto();
            if (userInfo != null)
            {
                var checkPass = PassExtension.VerifyPassword(userInfo.PasswordHash, request.Password);

                if (checkPass)
                {
                    var accessToken = await GenerateJSONWebToken(userInfo);
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        return null;
                    }

                    result.UserInfo = new AccountChildInfo();
                    result.UserInfo.Id = userInfo.Id;
                    result.UserInfo.UserName = userInfo.UserName;
                    result.AccessToken = accessToken;
                    result.CanAccess = true;
                }

            }

            return await Task.FromResult(result);
        }
        public async Task<AccountChildDto> Register(LoginRequestDto request)
        {
            var userInfo = new UserModel();
            if (!string.IsNullOrEmpty(request.UserName))
            {
                userInfo = (await _accountRepository.Select().ConfigureAwait(false))
                    .FirstOrDefault(x => x.UserName == request.UserName.Trim());
            }
            else if (!string.IsNullOrEmpty(request.Email))
            {
                userInfo = (await _accountRepository.Select().ConfigureAwait(false))
                    .FirstOrDefault(x => x.Email == request.Email.Trim());
            }
            var result = new AccountChildDto();
            if (userInfo == null && request.Password!= null)
            {
                userInfo = new UserModel();
                userInfo.PasswordHash = PassExtension.HashPassword(request.Password);
                userInfo.UserName = request.UserName;
                userInfo.Email = request.Email;
                userInfo = _unitOfWork.Merge(userInfo);
                var accessToken = await GenerateJSONWebTokenRegister(userInfo);
                if (string.IsNullOrEmpty(accessToken))
                {
                    return null;
                }

                result.UserInfo = new AccountChildInfo();
                result.UserInfo.Id = userInfo.Id;
                result.UserInfo.UserName = userInfo.UserName;
                result.AccessToken = accessToken;
                result.CanAccess = true;

            }

            return await Task.FromResult(result);
        }
        private async Task<string> GenerateJSONWebToken(UserModel userInfo)
        {
            var roles = await _roleRepository.SelectRoleByUserId(userInfo.Id).ConfigureAwait(false);


            var tz = TZConvert.GetTimeZoneInfo("SE Asia Standard Time");
          //  var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime cur = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            // get options
            var jwtAppSettingOptions = _config.GetSection("JwtIssuerOptions");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions["JwtKey"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = cur.AddDays(Convert.ToDouble(jwtAppSettingOptions["JwtExpireDays"]));

            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
            new Claim(JwtRegisteredClaimNames.Aud, _config["JwtIssuerOptions:JwtIssuer"]),
            new Claim(JwtRegisteredClaimNames.Iss, _config["JwtIssuerOptions:JwtIssuer"]),
                new Claim(ClaimTypes.Name, userInfo.UserName),
            //new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            new Claim(JwtRegisteredClaimNames.Jti, userInfo.Id)
 };
            claims.AddRange(roles.Select(role => new Claim("roles", role.Code)));
            var token = new JwtSecurityToken(
           issuer: _config["JwtIssuerOptions:JwtIssuer"],
           audience: _config["JwtIssuerOptions:JwtIssuer"],
           claims: claims,
           expires: expires,
           signingCredentials: credentials
           );


            //var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            //_config["Jwt:Issuer"],
            //claims,
            //expires: cur.AddDays(30),
            //signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<string> GenerateJSONWebTokenRegister(UserModel userInfo)
        {
            var role = _unitOfWork.Select<RoleEntity>().Where(x => x.Code == "COMMON").FirstOrDefault();
            var generalUser = _unitOfWork.Select<UserRoleEntity>().Where(x => x.RoleId == role.Id).FirstOrDefault();
            var userRole = new UserRoleEntity() { Id = Guid.NewGuid().ToString("N"), RoleId = generalUser.RoleId, UserId = userInfo.Id };
             _unitOfWork.Insert(userRole);
            var roles = await _roleRepository.SelectRoleByUserId(userInfo.Id).ConfigureAwait(false);


            var tz = TZConvert.GetTimeZoneInfo("SE Asia Standard Time");
     //       var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime cur = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            // get options
            var jwtAppSettingOptions = _config.GetSection("JwtIssuerOptions");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions["JwtKey"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = cur.AddDays(Convert.ToDouble(jwtAppSettingOptions["JwtExpireDays"]));

            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
            new Claim(JwtRegisteredClaimNames.Aud, _config["JwtIssuerOptions:JwtIssuer"]),
            new Claim(JwtRegisteredClaimNames.Iss, _config["JwtIssuerOptions:JwtIssuer"]),
                new Claim(ClaimTypes.Name, userInfo.UserName),
            //new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            new Claim(JwtRegisteredClaimNames.Jti, userInfo.Id)
 };
            claims.AddRange(roles.Select(role => new Claim("roles", role.Code)));
            var token = new JwtSecurityToken(
           issuer: _config["JwtIssuerOptions:JwtIssuer"],
           audience: _config["JwtIssuerOptions:JwtIssuer"],
           claims: claims,
           expires: expires,
           signingCredentials: credentials
           );


            //var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            //_config["Jwt:Issuer"],
            //claims,
            //expires: cur.AddDays(30),
            //signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}


        //private UserModel AuthenticateUser(UserModel request)
        //{
        //    var userInfo = new UserModel();
        //    if (!string.IsNullOrEmpty(request.UserName))
        //    {
        //        userInfo = (await _accountRepository.Select().ConfigureAwait(false))
        //            .FirstOrDefault(x => x.UserName == request.UserName.Trim());
        //    }
        //    else if (!string.IsNullOrEmpty(request.Email))
        //    {
        //        userInfo = (await _accountRepository.Select().ConfigureAwait(false))
        //            .FirstOrDefault(x => x.Email == request.Email.Trim());
        //    }
        //    var result = new AccountChildDto();
        //    if (userInfo != null)
        //    {
        //        var checkPass = PassExtension.VerifyPassword(userInfo.PasswordHash, request.Password);

        //        if (checkPass)
        //        {
        //            UserModel user = null;

        //            //Validate the User Credentials 
        //            //Demo Purpose, I have Passed HardCoded User Information 
        //            if (login.UserName == "Hello")
        //            {
        //                user = new UserModel { UserName = "Hello", Email = "giakhanhvy@gmail.com", Id = Guid.NewGuid().ToString("N") };
        //            }
        //            return user;
        //        }
        //    }
        //}

        //#endregion Public methods

        //#region Private methods
        //private void ClearTokenId(HttpContext httpContext)
        //{
        //    var cookieOptions = new CookieOptions() { Expires = DateTime.Now.AddMonths(-1) };
        //    httpContext.Response.Cookies.Delete("id-token", cookieOptions);
        //}

        //private void SetTokenId(HttpContext httpContext, string tokenId)
        //{
        //    var cookieOptions = new CookieOptions() { Expires = DateTime.Now.AddMonths(1) };
        //    httpContext.Response.Cookies.Append("id-token", tokenId, cookieOptions);
        //}

        //private string CombineUrl(string url1, string url2)
        //{
        //    if (!string.IsNullOrEmpty(url1) && !string.IsNullOrEmpty(url2))
        //    {
        //        string url1Trim = url1.TrimEnd('/');
        //        string url2Trim = url2.TrimStart('/');
        //        return string.Format("{0}/{1}", url1Trim, url2Trim);
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}
        
        //private string GenerateJwtToken(UserModel user)
        //{
        //    IdentityOptions _options = new IdentityOptions();
        //    // establish list of claim -- list of important information, each claim contains each infomation
        //    // add username, id, an encode factor
        //    var claims = new List<Claim>
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(ClaimTypes.Name, user.UserName)
        //    };

        //    // get options
        //    var jwtAppSettingOptions = _configuration.GetSection("JwtIssuerOptions");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions["JwtKey"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var expires = DateTime.Now.AddDays(Convert.ToDouble(jwtAppSettingOptions["JwtExpireDays"]));

        //    var token = new JwtSecurityToken(
        //        jwtAppSettingOptions["JwtIssuer"],
        //        jwtAppSettingOptions["JwtIssuer"],
        //        claims,
        //        expires: expires,
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

