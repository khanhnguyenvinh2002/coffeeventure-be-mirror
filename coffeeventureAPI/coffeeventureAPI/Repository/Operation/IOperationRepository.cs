using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OperationEntity = coffeeventureAPI.Data.Operation;
using ActionEntity = coffeeventureAPI.Data.Action;
using OperationActionEntity = coffeeventureAPI.Data.OperationAction;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Repository.Operation
{
    /// <summary>
    /// Interface repository
    /// </summary>
    public interface IOperationRepository : IScoped
    {
        Task<IEnumerable<OperationEntity>> GetMenyByUser(string id);
        Task<IQueryable<OperationEntity>> Select(OperationRequestDto request);
        Task<int> Count(OperationRequestDto request);
        Task<OperationDto> SelectById(string id);
        Task<OperationDto> Merge(OperationDto model);
        Task<bool> DeleteById(params string[] ids);
        Task<bool> BulkUpdate(IEnumerable<OperationDto> entities);
        Task<bool> BulkMergeAction(IEnumerable<OperationActionEntity> operationActions, string operationId);
        Task<bool> BulkDeleteByIds( string[] actionId, string operationId);
        
        Task<IQueryable<OperationEntity>> SelectByParentMenu(OperationRequestDto request);
        Task<IQueryable<ActionEntity>> SelectActionById(OperationRequestDto request);
    }
}
