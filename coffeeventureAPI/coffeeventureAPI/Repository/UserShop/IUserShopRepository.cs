using UserShopModel = coffeeventureAPI.Data.UserShop;
using ShopEntity = coffeeventureAPI.Data.Shop;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data.UserDto;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Repository.UserShop
{
    public interface IUserShopRepository : IScoped
    {
        Task<ShopEntity> SelectById(string id);
        Task<IEnumerable<ShopDto>> Select(UserShopRequestDto request);
        Task<IEnumerable<ShopDto>> GetAll();
        Task<int> Count(UserShopRequestDto request);
        //Task<UserShopModel> Merge(UserShopDto model);
        Task<bool> BulkInsertShop(IEnumerable<UserShopModel> ShopIds, string ShopId);
        Task<bool> InsertShop(UserShopModel shop, string ShopId);
        Task<UserShopModel> Update(UserShopModel model);
        Task<bool> Delete(string ShopId, string UserId);
    }
}
