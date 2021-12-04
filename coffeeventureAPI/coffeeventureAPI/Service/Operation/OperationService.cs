using OperationEntity = coffeeventureAPI.Data.Operation;
using OperationActionEntity = coffeeventureAPI.Data.OperationAction;
using ActionEntity = coffeeventureAPI.Data.Action;
using UserEntity = coffeeventureAPI.Data.User;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using coffeeventureAPI.Core.Service;
using coffeeventureAPI.Repository.Operation;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Service
{
    /// <summary>
    /// Service class
    /// </summary>
    public class OperationService : BaseService, IOperationService
    {
        #region Private variables

        private readonly IOperationRepository _operationRepository;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private variables

        #region Public methods

        public OperationService(IOperationRepository userRepository, IUnitOfWork unitOfWork) : base()
        {
            _operationRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<OperationEntity>> GetFlatMenyByUser()
        {
            var generalUser = _unitOfWork.Select<UserEntity>().Where(x => x.UserName == "general_user").FirstOrDefault();
            var operationsResult = await _operationRepository.GetMenyByUser(generalUser.Id);
            var operations = operationsResult.ToList();
            if (!string.IsNullOrEmpty(_unitOfWork.GetCurrentUserId()))
            {
                var or = await _operationRepository.GetMenyByUser(_unitOfWork.GetCurrentUserId());
                operations = or.ToList();
            }
            return await Task.FromResult(operations);
        }
        public async Task<IEnumerable<MenuDto>> GetMenyByUser()
        {
            var operationsResult = await _operationRepository.Select(new OperationRequestDto());
            var operations = operationsResult.ToList();
            if (!string.IsNullOrEmpty(_unitOfWork.GetCurrentUser().Id))
            {
                var or = await _operationRepository.GetMenyByUser(_unitOfWork.GetCurrentUser().Id);
                operations = or.ToList();

            }
            var operationSource = _unitOfWork.Select<OperationEntity>().ToList();

            var operationModules = new List<OperationEntity>();
            foreach (var operation in operations)
            {
                operationModules.AddRange(FindParent(operationSource, operation));
            }

            operations.AddRange(operationModules.Distinct());
            var result = operations.Where(x => x.ParentMenu == null).Select(x => new MenuDto(x)).ToList().OrderBy(x => x.Index);

            GetSub(operations, result);
            return result;
        }

        public async Task<bool> CheckUrlAuthGuard(string userId, string url)
        {
            var operationsResult = await _operationRepository.GetMenyByUser(userId);
            return operationsResult.Any(x => url.StartsWith(x.Link));
        }

        private List<OperationEntity> FindParent(List<OperationEntity> source, OperationEntity operation)
        {
            var result = new List<OperationEntity>();

            if (!string.IsNullOrEmpty(operation.ParentMenu))
            {
                var parent = source.Where(x => x.Id == operation.ParentMenu).SingleOrDefault();
                if (parent != null)
                {
                    result.Add(parent);
                    result.AddRange(FindParent(source, parent));
                }
            }

            return result;
        }

        public async Task<OperationDto> SelectById(string id)
        {
            return await _operationRepository.SelectById(id);
        }

        public async Task<IQueryable<OperationEntity>> Select(OperationRequestDto request)
        {
            return await _operationRepository.Select(request);
        }
        public async Task<IQueryable<OperationEntity>> SelectByParentMenu(OperationRequestDto request)
        {
            return await _operationRepository.SelectByParentMenu(request);
        }
        public async Task<IQueryable<ActionEntity>> SelectActionById(OperationRequestDto request)
        {
            return await _operationRepository.SelectActionById(request);
        }

        public async Task<int> Count(OperationRequestDto request)
        {
            return await _operationRepository.Count(request);
        }

        public async Task<OperationDto> Merge(OperationDto dto)
        {

            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();

            var dtoResult = await _operationRepository.Merge(dto);
            // Commit transaction
            transaction.Commit();
            return await _operationRepository.SelectById(dto.Id);
        }

        public async Task<bool> Delete(string id)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();

            var result = await _operationRepository.DeleteById(id);

            // Commit transaction
            transaction.Commit();

            return result;
        }
        public async Task<bool> BulkUpdate(IEnumerable<OperationDto> entities)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            bool check = await _operationRepository.BulkUpdate(entities);
            transaction.Commit();
            return check;
        }
        public async Task<bool> BulkMergeAction(OperationDto dto)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();
            // Merge role user
            var operationActions = dto.ActionId?.Select(x => new OperationActionEntity() { Id = Guid.NewGuid().ToString("N"), OperationId = dto.Id, ActionId = x }).ToArray();
            bool check = false;
            if (operationActions != null && operationActions.Length > 0)
            {
                check = await _operationRepository.BulkMergeAction(operationActions, dto.Id);
            }
            // Commit transaction
            transaction.Commit();
            return check;
        }

        public async Task<bool> BulkDeleteByIds(OperationDto dto)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();
            bool check = await _operationRepository.BulkDeleteByIds(dto.ActionId, dto.Id);
            // Commit transaction
            transaction.Commit();
            return check;
        }
        
        #endregion Public methods

        #region Private methods

        private void GetSub(IEnumerable<OperationEntity> source, IEnumerable<MenuDto> parents)
        {
            foreach (var parent in parents)
            {
                parent.Submenu = source.Where(x => x.ParentMenu == parent.Id).Select(x => new MenuDto(x)).OrderBy(x => x.Index).ToList();
                if (parent.Submenu.Count > 0)
                {
                    GetSub(source, parent.Submenu);
                }
                else
                {
                    parent.Submenu = null;
                }
            }
        }

        #endregion Private methods
    }
}
