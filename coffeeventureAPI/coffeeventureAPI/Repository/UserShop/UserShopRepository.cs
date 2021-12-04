
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

using UserShopEntity = coffeeventureAPI.Data.UserShop;
using ShopEntity = coffeeventureAPI.Data.Shop;
using ShopCategoryEntity = coffeeventureAPI.Data.ShopCategory;
using CategoryEntity = coffeeventureAPI.Data.Category;
using coffeeventureAPI.Model;
using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Core.Utilities;
using coffeeventureAPI.Data.UserDto;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Repository.UserShop
{
    public class UserShopRepository : BaseRepository, IUserShopRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        #region Constructor

        public UserShopRepository(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _config = configuration;
        }

        #endregion Constructor

        #region Public methods

        public async Task<ShopEntity> SelectById(string id)
        {
            IQueryable<ShopEntity> query2 = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            var UserShop = query2.Where(x => x.Id == id).SingleOrDefault();
            return await Task.FromResult(UserShop);
        }
        public async Task<IEnumerable<ShopDto>> GetAll()
        {
            IQueryable<UserShopEntity> query = _unitOfWork.Select<UserShopEntity>().AsNoTracking();
            IQueryable<ShopEntity> query2 = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            query = query.Where(x => x.UserId == _unitOfWork.GetCurrentUserId());
            var ids = query.Select(x => x.ShopId);
            var ans = query2.Where(x => ids.Contains(x.Id)).Select(x => new ShopDto() { Id =x.Id}).ToList();
            return await Task.FromResult(ans);
        }
        public async Task<IEnumerable<ShopDto>> Select(UserShopRequestDto request)
        {
            IQueryable<UserShopEntity> query = _unitOfWork.Select<UserShopEntity>().AsNoTracking();
            IQueryable<ShopEntity> query2 = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            var shopCategory = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking();
            var category = _unitOfWork.Select<CategoryEntity>().AsNoTracking();
            query = Filter(query, request);
            query = query.Paging(request);
            var ids = query.Select(x => x.ShopId);
            var ans = query2.Where(x => ids.Contains(x.Id)).Select(x=> new ShopDto(x)).ToList();
            foreach (var i in ans)
            {
                var shopCate = shopCategory.Where(x => x.ShopId == i.Id).Select(x => x.CategoryId);
                var cate = category.Where(x => shopCate.Contains(x.Id)).Select(x => new CategoryDtox(x) { });
                i.ShopCategory = cate;
            }
            return await Task.FromResult(ans);
        }

        public async Task<int> Count(UserShopRequestDto request)
        {
            IQueryable<UserShopEntity> query = _unitOfWork.Select<UserShopEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }

        public async Task<bool> BulkInsertShop(IEnumerable<UserShopEntity> shops, string ShopId)
        {
            var itemDelete = _unitOfWork.Select<UserShopEntity>().Where(x => x.ShopId == ShopId);
            _unitOfWork.BulkInsert(shops);
            return await Task.FromResult(true);
        }
        public async Task<bool> InsertShop(UserShopEntity shop, string ShopId)
        {
            var itemDelete = _unitOfWork.Select<UserShopEntity>().Where(x => x.ShopId == ShopId);
            _unitOfWork.Insert(shop);
            return await Task.FromResult(true);
        }
        public async Task<bool> Delete(string ShopId, string UserId)
        {
            var itemDelete = _unitOfWork.Select<UserShopEntity>().Where(x => x.UserId == UserId).Where(x => x.ShopId == ShopId);
            _unitOfWork.BulkDelete(itemDelete);
            return await Task.FromResult(true);
        }

        public async Task<UserShopEntity> Update(UserShopEntity model)
        {
            _unitOfWork.Update<UserShopEntity, UserShopEntity>(model);
            return await _unitOfWork.FindAsync<UserShopEntity>(model.Id);
        }


        #endregion Public methods

        #region Private methods

        private IQueryable<UserShopEntity> Filter(IQueryable<UserShopEntity> models, UserShopRequestDto request)
        {
            if (request.ShopIds != null && request.ShopIds.Any())
            {
                models = models.Where(x => request.ShopIds.Contains(x.ShopId));
            }
            if (!string.IsNullOrEmpty(request.ShopId))
            {
                models = models.Where(x => x.ShopId == request.ShopId);
            }
            if (!string.IsNullOrEmpty(_unitOfWork.GetCurrentUserId() ))
            {
                models = models.Where(x => _unitOfWork.GetCurrentUserId() == x.UserId);
            }
            //if (!string.IsNullOrEmpty(request.OrgId))
            //{
            //    models = models.Where(x => x.UserShopOrganization.Any(o => o.OrgId == request.OrgId));
            //}

          
            return models;
        }

        #endregion Private methods
    }
}
