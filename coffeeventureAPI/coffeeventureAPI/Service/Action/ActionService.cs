
using System;
using System.Linq;
using System.Threading.Tasks;
using ActionEntity = coffeeventureAPI.Data.Action;
using ActionInOutEntity = coffeeventureAPI.Data.ActionInOut;
using OperationActionEntity = coffeeventureAPI.Data.OperationAction;
using ResourceRestrictedEntity = coffeeventureAPI.Data.ResourceRestricted;
using System.Collections.Generic;
using coffeeventureAPI.Core.Service;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Repository.Action;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Service
{
    public class ActionService : BaseService, IActionService
    {
        #region Private variables

        private readonly IActionRepository _actionRepository;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private variables

        public ActionService(IActionRepository actionRepository, IUnitOfWork unitOfWork) : base()
        {
            _actionRepository = actionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IQueryable<ActionEntity>> Select(ActionRequestDto request)
        {
            return await _actionRepository.Select(request);
        }

        public async Task<int> Count(ActionRequestDto request)
        {
            return await _actionRepository.Count(request);
        }

        public async Task<IQueryable<ActionInOutEntity>> SelectActionInOut(ActionRequestDto request)
        {
            return await _actionRepository.SelectActionInOut(request);
        }

        public async Task<int> CountActionInOut(ActionRequestDto request)
        {
            return await _actionRepository.CountActionInOut(request);
        }

        public async Task<IEnumerable<ResourceDto>> SelectResouceByOperation(string operationId)
        {
            var result = new List<ResourceDto>();
            var actionIds = _unitOfWork.Select<OperationActionEntity>().Where(x => x.OperationId == operationId).Select(x => x.ActionId).ToList();
            var actions = _unitOfWork.Select<ActionEntity>().Where(x => actionIds.Contains(x.Id)).ToList();
            var actionInOuts = _unitOfWork.Select<ActionInOutEntity>().Where(x => actionIds.Contains(x.ActionId)).ToList();

            foreach (var action in actions)
            {
                result.Add(new ResourceDto()
                {
                    Id = action.Id,
                    Name = action.RoutePath,
                    Method = action.Method,
                    Type = 0 // 0: Action, 1: Action in/out
                });

                var inOuts = actionInOuts.Where(x => x.ActionId == action.Id);
                result.AddRange(inOuts.Select(x => new ResourceDto()
                {
                    Id = x.Id,
                    ParentId = action.Id,
                    Name = x.Name,
                    Method = x.InOutType,
                    Type = 1 // 0: Action, 1: Action in/out
                }).ToList());
            }

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<ResourceRestrictedEntity>> SelectResourceRestricted(MergeResourceRestrictedRequestDto request)
        {
            var result = _unitOfWork.Select<ResourceRestrictedEntity>()
                .Where(x => x.RoleId == request.RoleId && x.OperationId == request.OperationId);
            return await Task.FromResult(result);
        }

        public async Task<bool> MergeResourceRestricted(MergeResourceRestrictedRequestDto request)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            var restrictedDelete = _unitOfWork.Select<ResourceRestrictedEntity>()
                .Where(x => x.RoleId == request.RoleId && x.OperationId == request.OperationId).ToList();

            _unitOfWork.BulkDelete(restrictedDelete);
            _unitOfWork.BulkInsert(request.ResourceRestricted);

            transaction.Commit();

            return await Task.FromResult(true);
        }
    }
}
