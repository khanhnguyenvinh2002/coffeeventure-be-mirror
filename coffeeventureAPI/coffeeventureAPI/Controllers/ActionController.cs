
using coffeeventureAPI.Data;
using coffeeventureAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResourceRestrictedEntity = coffeeventureAPI.Data.ResourceRestricted;

namespace coffeeventureAPI.Controllers
{
    [Route("action")]
    public class ActionController : Controller
    {
        private readonly IActionService _actionService;
        public ActionController(IActionService actionService)
        {
            _actionService = actionService;
        }

        [HttpGet]
        public async Task<IEnumerable<Action>> Select([FromQuery] ActionRequestDto request)
        {
            return await _actionService.Select(request);
        }

        [Route("count")]
        [HttpGet]
        public async Task<int> Count([FromQuery] ActionRequestDto request)
        {
            return await _actionService.Count(request);
        }

        [Route("select-action-in-out")]
        [HttpGet]
        public async Task<IEnumerable<ActionInOut>> SelectActionInOut([FromQuery] ActionRequestDto request)
        {
            return await _actionService.SelectActionInOut(request);
        }

        [Route("select-resource-by-operation/{operationId}")]
        [HttpGet]
        public async Task<IEnumerable<ResourceDto>> SelectResouceByOperation([FromRoute] string operationId)
        {
            return await _actionService.SelectResouceByOperation(operationId);
        }

        [Route("select-resource-restricted")]
        [HttpGet]
        public async Task<IEnumerable<ResourceRestrictedEntity>> SelectResourceRestricted([FromQuery] MergeResourceRestrictedRequestDto request)
        {
            return await _actionService.SelectResourceRestricted(request);
        }

        [Route("merge-resource-restricted")]
        [HttpPost]
        public async Task<bool> MergeResourceRestricted([FromBody] MergeResourceRestrictedRequestDto request)
        {
            return await _actionService.MergeResourceRestricted(request);
        }

        [Route("count-action-in-out")]
        [HttpGet]
        public async Task<int> CountActionInOut([FromQuery] ActionRequestDto request)
        {
            return await _actionService.CountActionInOut(request);
        }
    }
}
