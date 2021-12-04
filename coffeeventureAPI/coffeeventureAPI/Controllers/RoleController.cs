using coffeeventureAPI.Data;
using coffeeventureAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using OperationEntity = coffeeventureAPI.Data.Operation;

namespace coffeeventureAPI.Controllers
{
    [Route("role")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Route("{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<RoleDto> SelectById([FromRoute] string id)
        {
            return await _roleService.SelectById(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<Role>> Select([FromQuery] RoleRequestDto request)
        {
            return await _roleService.Select(request);
        }

        [Route("count")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<int> Count([FromQuery] RoleRequestDto request)
        {
            return await _roleService.Count(request);
        }

        [Route("merge")]
        [HttpPost]
        public async Task<RoleDto> Merge([FromBody] RoleDto requestDto)
        {
            return await _roleService.Merge(requestDto);
        }

        [Route("merge-role-operation/{roleId}")]
        [HttpPost]
        public async Task<bool> BulkMergeRoleOperation([FromBody] IEnumerable<string> operationIds, [FromRoute] string roleId)
        {
            return await _roleService.BulkMergeRoleOperation(operationIds, roleId);
        }

        [Route("select-role-operation/{roleId}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<string[]> SelectRoleOperation([FromRoute] string roleId)
        {
            return await _roleService.SelectRoleOperation(roleId);
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<bool> Delete([FromRoute] string id)
        {
            return await _roleService.Delete(id);
        }

        [Route("select-menu-operation/{roleId}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<OperationEntity>> SelectMenuOperation([FromRoute] string roleId)
        {
            return await _roleService.SelectMenuOperation(roleId);
        }

    }
}
