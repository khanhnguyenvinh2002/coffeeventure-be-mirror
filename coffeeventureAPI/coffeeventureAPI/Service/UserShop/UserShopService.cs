using UserShopModel = coffeeventureAPI.Data.UserShop;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using coffeeventureAPI.Core.Service;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Data.UserDto;
using coffeeventureAPI.Repository.UserShop;
using ShopEntity = coffeeventureAPI.Data.Shop;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Service
{
    public class UserShopService : BaseService, IUserShopService
    {
        #region Private variables

        private readonly IUserShopRepository _userShopRepository;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private variables

        #region Public methods

        public UserShopService(IUserShopRepository UserShopRepository, IUnitOfWork unitOfWork) : base()
        {
            _userShopRepository = UserShopRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ShopEntity> SelectById(string id)
        {
            return await _userShopRepository.SelectById(id);
        }

        public async Task<IEnumerable<ShopDto>> Select(UserShopRequestDto request)
        {
            return await _userShopRepository.Select(request);
        }
        public async Task<IEnumerable<ShopDto>> GetAll()
        {
            return await _userShopRepository.GetAll();
        }
        public async Task<int> Count(UserShopRequestDto request)
        {
            return await _userShopRepository.Count(request);
        }

        public async Task<bool> Delete(UserShopRequestDto request)
        {
            return await _userShopRepository.Delete(request.ShopId, _unitOfWork.GetCurrentUserId());
        }
        public async Task<bool> BulkInsertShop(IEnumerable<string> ShopIds, string ShopId)
        {
            // Merge Shop UserShop
            var shops = ShopIds.Select(x => new UserShopModel() { Id = Guid.NewGuid().ToString("N"), ShopId = ShopId, UserId = x }).ToArray();
            return await _userShopRepository.BulkInsertShop(shops, ShopId);
        }
        public async Task<bool> InsertShop(string userId, string ShopId)
        {
            // Merge Shop UserShop
            var shop =  new UserShopModel() { Id = Guid.NewGuid().ToString("N"), ShopId = ShopId, UserId = userId };
            return await _userShopRepository.InsertShop(shop, ShopId);
        }
        //public async Task<UserShopDto> Merge(UserShopDto dto, UserShopRequestMergeDto requestMergeDto)
        //{
        //    // Begin transaction
        //    using var transaction = _unitOfWork.BeginTransaction();

        //    // Merge UserShop
        //    var dtoResult = await _userShopRepository.Merge(dto);

        //    // Merge Shop UserShop
        //    var Shops = dto.UserShopShop?.Select(x => new UserShopShopEntity() { Id = Guid.NewGuid().ToString("N"), ShopId = x.Id, UserShopId = dtoResult.Id }).ToArray();
        //    if (Shops != null && Shops.Length > 0)
        //    {
        //        await _userShopRepository.BulkMergeShop(Shops, dtoResult.Id);
        //    }

        //    // Merge UserShop organization
        //    var orgs = requestMergeDto.OrgIds?.Select(x => new UserShopOrganizationEntity() { Id = Guid.NewGuid().ToString("N"), UserShopId = dtoResult.Id, OrgId = x });
        //    if (requestMergeDto.IsUpdUserShopOrg)
        //    {
        //        await _userShopRepository.BulkMergeOrg(orgs, dtoResult.Id);
        //    }

        //    // Commit transaction
        //    transaction.Commit();
        //    return await _userShopRepository.SelectById(dto.Id);
        //}
        #endregion Public methods
    }
}
