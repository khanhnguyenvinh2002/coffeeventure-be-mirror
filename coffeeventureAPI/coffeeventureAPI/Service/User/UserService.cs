using UserModel = coffeeventureAPI.Data.User;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using UserOrganizationEntity = coffeeventureAPI.Data.UserOrganization;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using coffeeventureAPI.Core.Service;
using coffeeventureAPI.Repository.User;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Data.UserDto;
using Microsoft.AspNetCore.Http;

namespace coffeeventureAPI.Service
{
    public class UserService : BaseService, IUserService
    {
        #region Private variables

        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private variables

        #region Public methods

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : base()
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> SelectById(string id)
        {
            return await _userRepository.SelectById(id);
        }

        public async Task<IQueryable<UserModel>> Select(UserRequestSelectDto request)
        {
            return await _userRepository.Select(request);
        }

        public async Task<int> Count(UserRequestSelectDto request)
        {
            return await _userRepository.Count(request);
        }

        public async Task<bool> Delete(UserRequestSelectDto request)
        {
            return await _userRepository.Delete(request.RoleId, request.Id);
        }
        public async Task<bool> BulkInsertRole(IEnumerable<string> userIds, string roleId)
        {
            // Merge role user
            var roles = userIds.Select(x => new UserRoleEntity() { Id = Guid.NewGuid().ToString("N"), RoleId = roleId, UserId = x }).ToArray();
            return await _userRepository.BulkInsertRole(roles, roleId);
        }
        public async Task<UserDto> Merge(UserDto dto, UserRequestMergeDto requestMergeDto)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();

            // Merge user
            var dtoResult = await _userRepository.Merge(dto);

            // Merge role user
            var roles = dto.UserRole?.Select(x => new UserRoleEntity() { Id = Guid.NewGuid().ToString("N"), RoleId = x.Id, UserId = dtoResult.Id }).ToArray();
            if (roles != null && roles.Length > 0)
            {
                await _userRepository.BulkMergeRole(roles, dtoResult.Id);
            }

            // Merge user organization
            var orgs = requestMergeDto.OrgIds?.Select(x => new UserOrganizationEntity() { Id = Guid.NewGuid().ToString("N"), UserId = dtoResult.Id, OrgId = x });
            if (requestMergeDto.IsUpdUserOrg)
            {
                await _userRepository.BulkMergeOrg(orgs, dtoResult.Id);
            }

            // Commit transaction
            transaction.Commit();
            return await _userRepository.SelectById(dto.Id);
        }
        public async Task<bool> Upload(IFormFileCollection files)
        {

            foreach (var file in files)
            {
                await _userRepository.Upload(file);
            }
            return await Task.FromResult(true);
        }
        #endregion Public methods
    }
}
