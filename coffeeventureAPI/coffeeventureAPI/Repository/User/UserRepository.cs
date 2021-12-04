
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

using ImageEntity = coffeeventureAPI.Data.Image;
using UserEntity = coffeeventureAPI.Data.User;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using UserOrganizationEntity = coffeeventureAPI.Data.UserOrganization;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data.UserDto;
using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Core.Utilities;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http.Headers;
using System.Reflection;
using coffeeventureAPI.Service.Blob;

namespace coffeeventureAPI.Repository.User
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private string _rootPath = string.Empty;
        private IBlobService _blobService;

        #region Constructor

        public UserRepository(IUnitOfWork unitOfWork, IConfiguration configuration, IBlobService blobService)
        {
            _blobService = blobService;
            _unitOfWork = unitOfWork;
            _config = configuration;
            _rootPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.ToString() + _config["UploadLocation:MainPath"];
        }

        #endregion Constructor

        #region Public methods

        public async Task<UserDto> SelectById(string id)
        {
            IQueryable<UserEntity> query = _unitOfWork.Select<UserEntity>().AsNoTracking();
            var user = query.Include(x => x.UserRole)
               .ThenInclude(x => x.Role)
               //.Include(x => x.UserOrganization)
               .Where(x => x.Id == id)
               .Select(x => new UserDto(x)).SingleOrDefault();

            if (!string.IsNullOrEmpty(user.Avatar))
                user.AvatarPath = user.Avatar;
            return await Task.FromResult(user);
        }

        public async Task<IQueryable<UserEntity>> Select(UserRequestSelectDto request)
        {
            IQueryable<UserEntity> query = _unitOfWork.Select<UserEntity>().AsNoTracking();
            query = query.Include(x => x.UserRole);
            query = Filter(query, request).OrderBy(x => x.Email);
            query = query.Paging(request);
            return await Task.FromResult(query);
        }

        public async Task<int> Count(UserRequestSelectDto request)
        {
            IQueryable<UserEntity> query = _unitOfWork.Select<UserEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }
        public async Task<UserDto> Merge(UserDto model)
        {
            if (!string.IsNullOrEmpty(model.Password))
            {
                model.PasswordHash = PassExtension.HashPassword(model.Password);
            }
            var result = _unitOfWork.Merge<UserEntity, UserDto>(model);
            return await Task.FromResult(result);
        }

        public async Task<bool> BulkInsertRole(IEnumerable<UserRoleEntity> userRoles, string roleId)
        {
            var itemDelete = _unitOfWork.Select<UserRoleEntity>().Where(x => x.RoleId == roleId);
            _unitOfWork.BulkInsert(userRoles);
            return await Task.FromResult(true);
        }
        public async Task<bool> BulkMergeRole(IEnumerable<UserRoleEntity> userRoles, string userId)
        {
            var itemDelete = _unitOfWork.Select<UserRoleEntity>().Where(x => x.UserId == userId);
            _unitOfWork.BulkDelete(itemDelete);
            _unitOfWork.BulkInsert(userRoles);
            return await Task.FromResult(true);
        }
        public async Task<bool> Delete(string roleId, string userId)
        {
            var itemDelete = _unitOfWork.Select<UserRoleEntity>().Where(x => x.UserId == userId).Where(x => x.RoleId == roleId);
            _unitOfWork.BulkDelete(itemDelete);
            return await Task.FromResult(true);
        }

        public async Task<bool> BulkMergeOrg(IEnumerable<UserOrganizationEntity> useOrgs, string userId)
        {
            var itemDelete = _unitOfWork.Select<UserOrganizationEntity>().Where(x => x.UserId == userId);
            _unitOfWork.BulkDelete(itemDelete);
            if (useOrgs != null && useOrgs.Count() > 0)
            {
                _unitOfWork.BulkInsert(useOrgs);
            } 
            return await Task.FromResult(true);
        }
        public async Task<UserEntity> Update(UserDto model)
        {
            _unitOfWork.Update<UserEntity, UserDto>(model);
            return await _unitOfWork.FindAsync<UserEntity>(model.Id);
        }

        public async Task<UserEntity> Update(UserEntity model)
        {
            _unitOfWork.Update(model);
            return await _unitOfWork.FindAsync<UserEntity>(model.Id);
        }

        public async Task<bool> Upload(IFormFile file)
        {
            var entity = new ImageEntity();
            if (file.Length > 0)
            {
                var id = Guid.NewGuid().ToString("N");
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var result = await _blobService.UploadFileBlobAsync("coffeeventurecontainer", file.OpenReadStream(), file.ContentType, fileName);
                entity.Id = id;
                entity.Name = fileName;
                entity.Path = result.AbsoluteUri;
                entity.Size = (int?)file.Length;
               
            var user = _unitOfWork.Select<UserEntity>().Where(x => x.Id == _unitOfWork.GetCurrentUserId()).FirstOrDefault();
                user.Avatar = entity.Path;
                _unitOfWork.Insert(entity);
                _unitOfWork.Merge(user);
              
            }

            return await Task.FromResult(true);
        }

        #endregion Public methods

        #region Private methods

        private IQueryable<UserEntity> Filter(IQueryable<UserEntity> models, UserRequestSelectDto request)
        {
            if (request.Ids != null && request.Ids.Any())
            {
                models = models.Where(x => request.Ids.Contains(x.Id));
            }

            if (request.ExcludeIds != null && request.ExcludeIds.Length > 0)
            {
                models = models.Where(x => !request.ExcludeIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(request.UserName))
            {
                models = models.Where(x => x.UserName == request.UserName);
            }

            if (!string.IsNullOrEmpty(request.Id))
            {
                models = models.Where(x => x.Id == request.Id);
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                models = models.Where(x => x.Email.Contains(request.Email));
            }

            //if (!string.IsNullOrEmpty(request.OrgId))
            //{
            //    models = models.Where(x => x.UserOrganization.Any(o => o.OrgId == request.OrgId));
            //}

            if (!string.IsNullOrEmpty(request.RoleId))
            {
                models = models.Where(x => x.UserRole.Any(o => o.RoleId == request.RoleId));
            }

            if (!string.IsNullOrEmpty(request.FullName))
            {
                models = models.Where(x => x.UserName.ToLower().Trim().Contains(request.FullName) || x.FullName.ToLower().Trim().Contains(request.FullName));
            }

            if (!string.IsNullOrEmpty(request.GeneralFilter))
            {
                models = models.Where(x =>
                 x.UserName.ToLower().Trim().Contains(request.GeneralFilter.ToLower().Trim())
                || x.FullName.ToLower().Trim().Contains(request.GeneralFilter.ToLower().Trim()));
            }

            return models;
        }

        #endregion Private methods
    }
}
