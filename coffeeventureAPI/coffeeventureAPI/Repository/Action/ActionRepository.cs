
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ActionEntity = coffeeventureAPI.Data.Action;
using ActionInOutEntity = coffeeventureAPI.Data.ActionInOut;
using OperationActionEntity = coffeeventureAPI.Data.OperationAction;
using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Repository.Action
{
    public class ActionRepository : BaseRepository, IActionRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public ActionRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public async Task<IQueryable<ActionEntity>> Select(ActionRequestDto request)
        {
            IQueryable<ActionEntity> query = _unitOfWork.Select<ActionEntity>().AsNoTracking();
            query = query.OrderBy(x => x.Tag).ThenBy(x => x.Method).ThenBy(x => x.RoutePath);
            query = Filter(query, request);

            query = query.Paging(request);
            return await Task.FromResult(query);
        }

        public async Task<int> Count(ActionRequestDto request)
        {
            IQueryable<ActionEntity> query = _unitOfWork.Select<ActionEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }
        public async Task<IQueryable<ActionInOutEntity>> SelectActionInOut(ActionRequestDto request)
        {
            IQueryable<ActionInOutEntity> query = _unitOfWork.Select<ActionInOutEntity>().AsNoTracking();
            query = query.Where(x => x.ActionId == request.Id);
            query = query.OrderBy(x => x.InOutType).ThenBy(x => x.Name).ThenBy(x => x.DataType);
            query = query.Paging(request);
            return await Task.FromResult(query);
        }

        public async Task<int> CountActionInOut(ActionRequestDto request)
        {
            IQueryable<ActionInOutEntity> query = _unitOfWork.Select<ActionInOutEntity>().AsNoTracking();
            query = query.Where(x => x.ActionId == request.Id);
            query = query.OrderBy(x => x.InOutType).ThenBy(x => x.Name).ThenBy(x => x.DataType);
            query = query.Paging(request);
            return await query.CountAsync();
        }

        #region Private methods

        private IQueryable<ActionEntity> Filter(IQueryable<ActionEntity> models, ActionRequestDto request)
        {
            if (!string.IsNullOrEmpty(request.ExcludeOperationId))
            {
                IQueryable<OperationActionEntity> operationQuery = _unitOfWork.Select<OperationActionEntity>().AsNoTracking();
                var actionIds = operationQuery.Where(x => x.OperationId == request.ExcludeOperationId).Select(x => x.ActionId);
                models = models.Where(x => !actionIds.Contains(x.Id));
            }
            if (!string.IsNullOrEmpty(request.IncludeOperationId))
            {
                IQueryable<OperationActionEntity> operationQuery = _unitOfWork.Select<OperationActionEntity>().AsNoTracking();
                var actionIds = operationQuery.Where(x => x.OperationId == request.IncludeOperationId).Select(x => x.ActionId);
                models = models.Where(x => actionIds.Contains(x.Id));
            }
            if (!string.IsNullOrEmpty(request.RoutePath))
            {
                models = models.Where(x => x.RoutePath.Contains(request.RoutePath));
            }

            if (!string.IsNullOrEmpty(request.Method))
            {
                models = models.Where(x => x.Method.Contains(request.Method));
            }

            if (!string.IsNullOrEmpty(request.Tag))
            {
                models = models.Where(x => x.Tag.Contains(request.Tag));
            }

            return models;
        }

        #endregion Private methods
    }
}