
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserShopModel = coffeeventureAPI.Data.UserShop;
using ShopEntity = coffeeventureAPI.Data.Shop;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data.UserDto;
using coffeeventureAPI.Data;

namespace coffeeventureAPI.Service
{
    public interface IUserShopService : IScoped
    {
        Task<ShopEntity> SelectById(string id);
        Task<IEnumerable<ShopDto>> Select(UserShopRequestDto request);
        Task<IEnumerable<ShopDto>> GetAll();
        Task<bool> BulkInsertShop(IEnumerable<string> ShopIds, string shopId);
        Task<bool> InsertShop(string userShopId, string shopId);
        Task<int> Count(UserShopRequestDto request);
        Task<bool> Delete(UserShopRequestDto request);
       // Task<UserShopModel> Merge(UserShopModel dto, UserShopRequestDto requestMergeDto);
    }
}
