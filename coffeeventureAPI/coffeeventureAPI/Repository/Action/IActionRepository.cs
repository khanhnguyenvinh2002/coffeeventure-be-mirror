using ActionEntity = coffeeventureAPI.Data.Action;
using System.Linq;
using System.Threading.Tasks;
using ActionInOutEntity = coffeeventureAPI.Data.ActionInOut;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Repository.Action
{
    public interface IActionRepository : IScoped
    {
        Task<IQueryable<ActionEntity>> Select(ActionRequestDto request);
        Task<int> Count(ActionRequestDto request);
        Task<IQueryable<ActionInOutEntity>> SelectActionInOut(ActionRequestDto request);
        Task<int> CountActionInOut(ActionRequestDto request);
    }
}
