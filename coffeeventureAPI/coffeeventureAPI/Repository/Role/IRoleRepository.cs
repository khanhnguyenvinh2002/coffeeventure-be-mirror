using RoleEntity = coffeeventureAPI.Data.Role;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using RoleOperationEntity = coffeeventureAPI.Data.RoleOperation;
using OperationEntity = coffeeventureAPI.Data.Operation;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Repository.Role
{
    public interface IRoleRepository : IScoped
    {
        Task<RoleDto> SelectById(string id);
        Task<IQueryable<RoleEntity>> Select(RoleRequestDto request);
        Task<int> Count(RoleRequestDto request);
        Task<RoleDto> Merge(RoleDto model);
        Task<bool> BulkMergeRole(IEnumerable<UserRoleEntity> userRoles, string roleId);
        Task<bool> BulkMergeRoleOperation(IEnumerable<RoleOperationEntity> operations, string roleId);
        Task<bool> DeleteById(params string[] ids);
        Task<List<RoleEntity>> SelectRoleByUserId(string userId);
    }
}
