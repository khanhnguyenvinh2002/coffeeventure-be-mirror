
using System.Linq;
using System.Threading.Tasks;
using ActionModel = coffeeventureAPI.Data.Action;
using ActionInOutEntity = coffeeventureAPI.Data.ActionInOut;
using OperationActionEntity = coffeeventureAPI.Data.OperationAction;
using ResourceRestrictedEntity = coffeeventureAPI.Data.ResourceRestricted;
using System.Collections.Generic;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Service
{
    public interface IActionService : IScoped
    {
        Task<IQueryable<ActionModel>> Select(ActionRequestDto request);
        Task<int> Count(ActionRequestDto request);
        Task<IQueryable<ActionInOutEntity>> SelectActionInOut(ActionRequestDto request);
        Task<int> CountActionInOut(ActionRequestDto request);
        Task<IEnumerable<ResourceDto>> SelectResouceByOperation(string operationId);
        Task<IEnumerable<ResourceRestrictedEntity>> SelectResourceRestricted(MergeResourceRestrictedRequestDto request);
        Task<bool> MergeResourceRestricted(MergeResourceRestrictedRequestDto request);
    }
}
