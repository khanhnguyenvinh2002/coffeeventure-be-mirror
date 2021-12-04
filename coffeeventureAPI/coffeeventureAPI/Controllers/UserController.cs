using coffeeventureAPI.Data;
using coffeeventureAPI.Data.UserDto;
using coffeeventureAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coffeeventureAPI.Controllers
{
    [Route("user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("count")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<int> Count([FromQuery] UserRequestSelectDto request)
        {
            return await _userService.Count(request);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<User>> Select([FromQuery] UserRequestSelectDto request)
        {
            return await _userService.Select(request);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> SelectById([FromRoute] string id)
        {
            var result = await _userService.SelectById(id);
            return Ok(result);
        }
        [HttpGet]
        [Route("view-user/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ViewUsertById([FromRoute] string id)
        {
            var result = await _userService.SelectById(id);
            return Ok(new UserDto() { Id = result.Id, UserName = result.UserName, AvatarPath = result.AvatarPath});
        }

        [Route("merge")]
        [HttpPost]
        public async Task<UserDto> Merge([FromBody] UserDto requestDto, [FromQuery] UserRequestMergeDto requestMergeDto)
        {
            return await _userService.Merge(requestDto, requestMergeDto);
        }

        [Route("bulk-insert")]
        [HttpPost]
        public async Task<bool> BulkInsertRole([FromBody] UserRequestSelectDto userRoles)
        {
            return await _userService.BulkInsertRole(userRoles.Ids, userRoles.RoleId);
        }

        [Route("delete-user-role")]
        [HttpDelete]
        public async Task<bool> Delete([FromQuery] UserRequestSelectDto request)
        {
            return await _userService.Delete(request);
        }

        [Route("upload-avatar")]
        [HttpPost]
        public async Task<bool> Upload()
        {
            //if (Request.Headers["id-token"].Count() > 0)
            //{
            //    var stream = Request.Headers["id-token"];
            //    var handler = new JwtSecurityTokenHandler();
            //    var jsonToken = handler.ReadToken(stream);
            //    var tokenS = jsonToken as JwtSecurityToken;
            //    var jti = tokenS.Claims.First(claim => claim.Type == "jti").Value;
            //    _unitOfWork.SetCurrentUserId(jti);
            //}
            return await _userService.Upload(Request.Form.Files);
        }

    }
}
