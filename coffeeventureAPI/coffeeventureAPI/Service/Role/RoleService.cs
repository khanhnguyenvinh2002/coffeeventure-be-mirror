using System;
using System.Linq;
using System.Threading.Tasks;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using RoleModel = coffeeventureAPI.Data.Role;
using RoleOperationEntity = coffeeventureAPI.Data.RoleOperation;
using OperationEntity = coffeeventureAPI.Data.Operation;
using System.Collections.Generic;
using coffeeventureAPI.Data;
using coffeeventureAPI.Core.Service;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Repository.Role;

namespace coffeeventureAPI.Service
{
    public class RoleService : BaseService, IRoleService
    {
        #region Private variables

        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private variables

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : base()
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleDto> SelectById(string id)
        {
            return await _roleRepository.SelectById(id);
        }

        public async Task<IQueryable<RoleModel>> Select(RoleRequestDto request)
        {
            return await _roleRepository.Select(request);
        }

        public async Task<int> Count(RoleRequestDto request)
        {
            return await _roleRepository.Count(request);
        }

        public async Task<RoleDto> Merge(RoleDto dto)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();

            // Merge role
            var dtoResult = await _roleRepository.Merge(dto);

            // Merge role user
            var roles = dto.User?.Select(x => new UserRoleEntity() { Id = Guid.NewGuid().ToString("N"), RoleId = dtoResult.Id, UserId = x.Id }).ToArray();
            if (roles != null && roles.Length > 0)
            {
                await _roleRepository.BulkMergeRole(roles, dtoResult.Id);
            }
            // Commit transaction
            transaction.Commit();
            return await _roleRepository.SelectById(dto.Id);
        }

        public async Task<bool> BulkMergeRoleOperation(IEnumerable<string> operationIds, string roleId)
        {
            var roleOperations = new List<RoleOperationEntity>();
            foreach (var operationId in operationIds)
            {
                roleOperations.Add(new RoleOperationEntity() { Id = Guid.NewGuid().ToString("N"), RoleId = roleId, OperationId = operationId });
            }
            return await _roleRepository.BulkMergeRoleOperation(roleOperations, roleId);
        }

        public async Task<string[]> SelectRoleOperation(string roleId)
        {
            var result = _unitOfWork.Select<RoleOperationEntity>().Where(x => x.RoleId == roleId).Select(x => x.OperationId).ToArray();
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<OperationEntity>> SelectMenuOperation(string roleId)
        {
            var operationIds = _unitOfWork.Select<RoleOperationEntity>().Where(x => x.RoleId == roleId).Select(x => x.OperationId).ToList();
            var operationSource = _unitOfWork.Select<OperationEntity>().ToList();
            var operations = _unitOfWork.Select<OperationEntity>().Where(x => operationIds.Contains(x.Id)).ToList();
            var operationModules = new List<OperationEntity>();
            foreach (var operation in operations)
            {
                operationModules.AddRange(FindParent(operationSource, operation));
            }

            operations.AddRange(operationModules.Distinct());

            return await Task.FromResult(operations);
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

        public async Task<bool> Delete(string id)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            var result = await _roleRepository.DeleteById(id);
            transaction.Commit();
            return result;
        }
    }
}
