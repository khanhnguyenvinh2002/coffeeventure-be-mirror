
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using UserModel = coffeeventureAPI.Data.User;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data.UserDto;
using Microsoft.AspNetCore.Http;

namespace coffeeventureAPI.Service
{
    public interface IUserService : IScoped
    {
        Task<UserDto> SelectById(string id);
        Task<IQueryable<UserModel>> Select(UserRequestSelectDto request);
        Task<bool> BulkInsertRole(IEnumerable<string> userIds, string roleId);
        Task<int> Count(UserRequestSelectDto request);
        Task<bool> Delete(UserRequestSelectDto request);
        Task<UserDto> Merge(UserDto dto, UserRequestMergeDto requestMergeDto);
        Task<bool> Upload(IFormFileCollection files);
    }
}
