
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using OperationEntity = coffeeventureAPI.Data.Operation;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data;

using ActionEntity = coffeeventureAPI.Data.Action;
namespace coffeeventureAPI.Service
{
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IOperationService : IScoped
    {
        Task<IEnumerable<OperationEntity>> GetFlatMenyByUser();
        Task<OperationDto> SelectById(string id);
        Task<IQueryable<OperationEntity>> Select(OperationRequestDto request);
        Task<IQueryable<OperationEntity>> SelectByParentMenu(OperationRequestDto request);
        Task<IQueryable<ActionEntity>> SelectActionById(OperationRequestDto request);
        Task<int> Count(OperationRequestDto request);
        Task<OperationDto> Merge(OperationDto dto);
        Task<bool> Delete(string id);
        Task<bool> BulkUpdate(IEnumerable<OperationDto> entities);
        Task<bool> BulkMergeAction(OperationDto dto);
        Task<bool> BulkDeleteByIds(OperationDto dto);
        Task<bool> CheckUrlAuthGuard(string userId, string url);
    }
}
