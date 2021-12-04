
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RoleEntity = coffeeventureAPI.Data.Role;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using RoleOperationEntity = coffeeventureAPI.Data.RoleOperation;
using OperationEntity = coffeeventureAPI.Data.Operation;
using coffeeventureAPI.Data;
using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Model;

namespace coffeeventureAPI.Repository.Role
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        #region Constructor

        public RoleRepository(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        #endregion

        public async Task<RoleDto> SelectById(string id)
        {
            IQueryable<RoleEntity> query = _unitOfWork.Select<RoleEntity>().AsNoTracking();
            var role = query
                .Where(x => x.Id == id)
                .Select(x => new RoleDto(x)).SingleOrDefault();
            return await Task.FromResult(role);
        }

        public async Task<IQueryable<RoleEntity>> Select(RoleRequestDto request)
        {
            IQueryable<RoleEntity> query = _unitOfWork.Select<RoleEntity>().AsNoTracking();
            query = Filter(query, request).OrderBy(x => x.Name);
            query = query.Paging(request);
            return await Task.FromResult(query);
        }

        public async Task<int> Count(RoleRequestDto request)
        {
            IQueryable<RoleEntity> query = _unitOfWork.Select<RoleEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }

        public async Task<RoleDto> Merge(RoleDto model)
        {
            var result = _unitOfWork.Merge<RoleEntity, RoleDto>(model);
            return await Task.FromResult(result);
        }

        public async Task<bool> BulkMergeRole(IEnumerable<UserRoleEntity> userRoles, string roleId)
        {
            var itemDelete = _unitOfWork.Select<UserRoleEntity>().Where(x => x.RoleId == roleId);
            _unitOfWork.BulkDelete(itemDelete);
            _unitOfWork.BulkInsert(userRoles);
            return await Task.FromResult(true);
        }

        public async Task<bool> BulkMergeRoleOperation(IEnumerable<RoleOperationEntity> operations, string roleId)
        {
            var itemDelete = _unitOfWork.Select<RoleOperationEntity>().Where(x => x.RoleId == roleId);
            _unitOfWork.BulkDelete(itemDelete);
            _unitOfWork.BulkInsert(operations);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteById(params string[] ids)
        {
            var roleDelete = _unitOfWork.Select<UserRoleEntity>().Where(x => ids.Contains(x.RoleId));
            _unitOfWork.BulkDelete(roleDelete);
            _unitOfWork.Delete<RoleEntity>(ids);
            return await Task.FromResult(true);
        }

        public async Task<List<RoleEntity>> SelectRoleByUserId(string userId)
        {
            var roleIds = _unitOfWork.Select<UserRoleEntity>().Where(x => x.UserId == userId)
                .Select(x => x.RoleId).Distinct();

            IQueryable<RoleEntity> query = _unitOfWork.Select<RoleEntity>().AsNoTracking();
            var listRole = new List<RoleEntity>();
            if (roleIds != null)
            {
                foreach (var item in roleIds)
                {
                    var role = query.Where(x => x.Id == item).SingleOrDefault();
                    if (role != null)
                    {
                        listRole.Add(role);
                    }
                }
            }
            return await Task.FromResult(listRole);
        }
            #region Private methods

            private IQueryable<RoleEntity> Filter(IQueryable<RoleEntity> models, RoleRequestDto searchEntity)
        {
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.Id == searchEntity.Id);
            }

            if (!string.IsNullOrEmpty(searchEntity.Name))
            {
                models = models.Where(x => x.Name.Contains(searchEntity.Name));
            }

            return models;
        }

        #endregion Private methods
    }
}