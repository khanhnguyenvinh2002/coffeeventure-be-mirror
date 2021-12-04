using UserModel = coffeeventureAPI.Data.User;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using UserOrganizationEntity = coffeeventureAPI.Data.UserOrganization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data.UserDto;
using Microsoft.AspNetCore.Http;

namespace coffeeventureAPI.Repository.User
{
    public interface IUserRepository : IScoped
    {
        Task<UserDto> SelectById(string id);
        Task<IQueryable<UserModel>> Select(UserRequestSelectDto request);
        Task<int> Count(UserRequestSelectDto request);
        Task<UserDto> Merge(UserDto model);
        Task<bool> BulkInsertRole(IEnumerable<UserRoleEntity> userRoles, string roleId);
        Task<bool> BulkMergeRole(IEnumerable<UserRoleEntity> userRoles, string roleId);
        Task<bool> BulkMergeOrg(IEnumerable<UserOrganizationEntity> useOrgs, string userId);
        Task<UserModel> Update(UserDto model);
        Task<bool> Upload(IFormFile file);
        Task<UserModel> Update(UserModel model);
        Task<bool> Delete(string roleId, string userId);
    }
}
