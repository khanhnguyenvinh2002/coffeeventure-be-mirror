using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleModel = coffeeventureAPI.Data.Role;
using OperationEntity = coffeeventureAPI.Data.Operation;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Service
{
    public interface IRoleService : IScoped
    {
        Task<RoleDto> SelectById(string id);
        Task<IQueryable<RoleModel>> Select(RoleRequestDto request);
        Task<int> Count(RoleRequestDto request);
        Task<RoleDto> Merge(RoleDto dto);
        Task<bool> BulkMergeRoleOperation(IEnumerable<string> operationIds, string roleId);
        Task<string[]> SelectRoleOperation(string roleId);
        Task<bool> Delete(string id);
        Task<IEnumerable<OperationEntity>> SelectMenuOperation(string roleId);
    }
}
