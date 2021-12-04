using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using OperationEntity = coffeeventureAPI.Data.Operation;
using ActionEntity = coffeeventureAPI.Data.Action;
using coffeeventureAPI.Service;
using coffeeventureAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace coffeeventureAPI.Controllers
{
    [Route("operation")]
    [Authorize]
    public class OperationController : Controller
    {
        private readonly IOperationService _operationService;
        public OperationController(IOperationService operationService)
        {
            _operationService = operationService;
        }

        [AllowAnonymous]
        [Route("get-menu-by-user")]
        [HttpGet]
        public async Task<IEnumerable<OperationEntity>> GetFlatMenyByUser()
        {
            return await _operationService.GetFlatMenyByUser();
        }

        [Route("{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<OperationDto> SelectById([FromRoute] string id)
        {
            return await _operationService.SelectById(id);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<OperationEntity>> Select([FromQuery] OperationRequestDto request)
        {
            return await _operationService.Select(request);
        }

        [Route("get-menu-by-parent-menu")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<OperationEntity>> SelectByParentMenu([FromQuery] OperationRequestDto request)
        {
            return await _operationService.SelectByParentMenu(request);
        }

        [Route("get-action-by-operation")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ActionEntity>> SelectActionById([FromQuery] OperationRequestDto request)
        {
            return await _operationService.SelectActionById(request);
        }

        [Route("count")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<int> Count([FromQuery] OperationRequestDto request)
        {
            return await _operationService.Count(request);
        }

        [Route("merge")]
        [HttpPost]
        public async Task<OperationDto> Merge([FromBody] OperationDto requestDto)
        {
            return await _operationService.Merge(requestDto);
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<bool> Delete([FromRoute] string id)
        {
            return await _operationService.Delete(id);
        }
        [Route("bulk-update")]
        [HttpPut]
        public async Task<bool> BulkUpdate([FromBody] IEnumerable<OperationDto> entities)
        {
            return await _operationService.BulkUpdate(entities);
        }

        [Route("bulk-merge-action")]
        [HttpPost]
        public async Task<bool> BulkMergeAction([FromBody] OperationDto dto)
        {
            return await _operationService.BulkMergeAction(dto);

        }

        [Route("bulk-delete")]
        [HttpPost]
        public async Task<bool> BulkDeleteByIds([FromBody] OperationDto dto)
        {
            return await _operationService.BulkDeleteByIds(dto);

        }
    }
}
