using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OperationEntity = coffeeventureAPI.Data.Operation;
using RoleOperationEntity = coffeeventureAPI.Data.RoleOperation;
using OperationActionEntity = coffeeventureAPI.Data.OperationAction;
using ActionEntity = coffeeventureAPI.Data.Action;
using UserEntity = coffeeventureAPI.Data.User;
using RoleEntity = coffeeventureAPI.Data.Role;
using UserRoleEntity = coffeeventureAPI.Data.UserRole;
using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Data;
using coffeeventureAPI.Model;
using System.Data.SqlClient;
using System.Data;
using System;
using Microsoft.Extensions.Logging;

namespace coffeeventureAPI.Repository.Operation
{
    /// <summary>
    /// Class repository
    /// </summary>
    public class OperationRepository : BaseRepository, IOperationRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly ILogger<OperationRepository> _logger;
        #region Constructor

        public OperationRepository(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<OperationRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _config = configuration;
            _logger = logger;
        }

        #endregion Constructor


        #region Public methods
    //    SELECT o.Id, o.Name, o.Link, o.MenuIcon, o.ParentMenu, o.MenuOrder FROM (
    //    SELECT ro.OperationId
    //    FROM [dbo].[user] u JOIN [dbo].[user_role] ur ON u.Id = ur.UserId

    //                    JOIN[dbo].[role_operation] ro ON ro.RoleId = ur.RoleId

    //    WHERE u.Id = @UserId

    //    UNION
    //    SELECT ro.OperationId FROM [dbo].[role_operation] ro WHERE ro.RoleId = @UserId
    //) AS temp JOIN operation o ON temp.OperationId = o.Id
        public async Task<IEnumerable<OperationEntity>> GetMenyByUser(string id)
        {
            var operation = _unitOfWork.Select<OperationEntity>().AsNoTracking();
            var roleOperation = _unitOfWork.Select<RoleOperationEntity>().AsNoTracking();
            var user = _unitOfWork.Select<UserEntity>().AsNoTracking();
            var userRole = _unitOfWork.Select<UserRoleEntity>().AsNoTracking();
            var role = _unitOfWork.Select<RoleEntity>().AsNoTracking();
            var query = user.Where(x => x.Id == id).Select(x=> x.Id).FirstOrDefault();
            var roleId = userRole.Where(x => x.UserId == query).Select(x=> x.RoleId).ToList();
            string[] ids = new string[] { };
            foreach (var i in roleId)
            {
                var ro = roleOperation.Where(x => x.RoleId == i).Select(x => x.OperationId);
                ids = ids.Concat(operation.Where(x => ro.Contains(x.Id)).Select(x=>x.Id).ToArray()).ToArray();
            }
            var ans = operation.Where(x => ids.Contains(x.Id)).OrderBy(x=>x.MenuOrder).ToList();


            //var ans = (from u in user
            //           join ur in userRole on u.Id equals ur.UserId
            //           join ro in roleOperation on ur.RoleId equals ro.RoleId where u.Id == id select ro.OperationId);
            //var answer = (from a in ans
            //             join o in operation on a.OperationId = o.Id select new OperationEntity() {Id = o.Id, Name = o.Name,Link = o.Link, ParentMenu = o.ParentMenu,MenuOrder = o.MenuOrder,MenuIcon =o.MenuIcon });
            //var result = answer;
            return await Task.FromResult(ans);
         
        }

        public async Task<OperationDto> SelectById(string id)
        {
            IQueryable<OperationEntity> query = _unitOfWork.Select<OperationEntity>().AsNoTracking();
            var role = query.Include(x => x.ParentMenuNavigation)
                .Where(x => x.Id == id)
                .Select(x => new OperationDto(x)).SingleOrDefault();
            return await Task.FromResult(role);
        }

        public async Task<IQueryable<OperationEntity>> Select(OperationRequestDto request)
        {
            IQueryable<OperationEntity> query = _unitOfWork.Select<OperationEntity>().AsNoTracking();
            query = Filter(query, request);
            query = query.Paging(request);
            return await Task.FromResult(query);
        }
        public async Task<IQueryable<OperationEntity>> SelectByParentMenu(OperationRequestDto request)
        {
            IQueryable<OperationEntity> query = _unitOfWork.Select<OperationEntity>().AsNoTracking();
            query = FilterByParentMenu(query, request);
            query = query.Paging(request);
            return await Task.FromResult(query);
        }
        public async Task<IQueryable<ActionEntity>> SelectActionById(OperationRequestDto request)
        {
            //get list of action by operation
            IQueryable<OperationActionEntity> query = _unitOfWork.Select<OperationActionEntity>().AsNoTracking();

            //get action ids from list
            var actionIds = FilterActionById(query, request).Select(x => x.ActionId);

            //get action entity by id
            IQueryable<ActionEntity> actionQuery = _unitOfWork.Select<ActionEntity>().AsNoTracking();
            actionQuery = actionQuery.Where(x => actionIds.Contains(x.Id));
            return await Task.FromResult(actionQuery);
        }

        public async Task<int> Count(OperationRequestDto request)
        {
            IQueryable<OperationEntity> query = _unitOfWork.Select<OperationEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }

        public async Task<OperationDto> Merge(OperationDto model)
        {
            var result = _unitOfWork.Merge<OperationEntity, OperationDto>(model);
            return await Task.FromResult(result);
        }

        public async Task<bool> DeleteById(params string[] id)
        {
            //get children
            string[] ids = GetChildren(id);

            //delete operation entity
            var childDelete = _unitOfWork.Select<OperationEntity>().Where(x => ids.Contains(x.Id));
            _unitOfWork.BulkDelete(childDelete);

            //delete operation action
            var actionDelete = _unitOfWork.Select<OperationActionEntity>().Where(x => ids.Contains(x.OperationId));
            _unitOfWork.BulkDelete(actionDelete);
            return await Task.FromResult(true);
        }
        public async Task<bool> BulkUpdate(IEnumerable<OperationDto> entities)
        {
            _unitOfWork.BulkUpdate<OperationEntity, OperationDto>(entities);
            return await Task.FromResult(true);
        }

        public async Task<bool> BulkMergeAction(IEnumerable<OperationActionEntity> operationActions, string operationId)
        {
            //delete old action
            //var itemDelete = _unitOfWork.Select<OperationActionEntity>().Where(x => x.OperationId == operationId);
            //_unitOfWork.BulkDelete(itemDelete);

            //only merge new actions, not delete old actions
            _unitOfWork.BulkInsert(operationActions);
            return await Task.FromResult(true);
        }
        public async Task<bool> BulkDeleteByIds(string[] actionId, string operationId)
        {
            //delete specific action
            var itemDelete = _unitOfWork.Select<OperationActionEntity>().Where(x => x.OperationId == operationId).Where(x => actionId.Contains(x.ActionId));
            _unitOfWork.BulkDelete(itemDelete);
            return await Task.FromResult(true);
        }
        
        #endregion Public methods

        #region Private methods

        private string[] GetChildren(params string[] ids)
        {
            var childDelete = _unitOfWork.Select<OperationEntity>().Where(x => ids.Contains(x.ParentMenu)).Select(x => x.Id).ToArray();
            if (childDelete.Length == 0 || childDelete == null)
            {
                return ids;
            }
            return ids.Concat(GetChildren(childDelete)).ToArray();
        }

        private IQueryable<OperationEntity> Filter(IQueryable<OperationEntity> models, OperationRequestDto searchEntity)
        {
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.Id == searchEntity.Id);
            }

            if (searchEntity.ExcludeIds != null && searchEntity.ExcludeIds.Length > 0)
            {
                models = models.Where(x => !searchEntity.ExcludeIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(searchEntity.Name))
            {
                models = models.Where(x => x.Name.Contains(searchEntity.Name));
            }

            if (!string.IsNullOrEmpty(searchEntity.ParentMenu))
            {
                models = models.Where(x => x.ParentMenu == searchEntity.ParentMenu);
            }

            if (!string.IsNullOrEmpty(searchEntity.Method))
            {
                models = models.Where(x => x.Method == searchEntity.Method);
            }

            if (searchEntity.Type != null)
            {
                models = models.Where(x => x.Type == searchEntity.Type);
            }

            return models.OrderBy(x => x.MenuOrder);
        }
        private IQueryable<OperationEntity> FilterByParentMenu(IQueryable<OperationEntity> models, OperationRequestDto searchEntity)
        {
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.Id == searchEntity.Id);
            }

            if (!string.IsNullOrEmpty(searchEntity.Name))
            {
                models = models.Where(x => x.Name.Contains(searchEntity.Name));
            }
            models = models.Where(x => x.ParentMenu == searchEntity.ParentMenu);

            return models.OrderBy(x => x.MenuOrder);
        }
        private IQueryable<OperationActionEntity> FilterActionById(IQueryable<OperationActionEntity> models, OperationRequestDto searchEntity)
        {
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.OperationId == searchEntity.Id);
            }

            if (searchEntity.ExcludeIds != null && searchEntity.ExcludeIds.Length > 0)
            {
                models = models.Where(x => !searchEntity.ExcludeIds.Contains(x.OperationId));
            }

            return models.OrderBy(x => x.ActionId);
        }


        #endregion Private methods
    }
}
