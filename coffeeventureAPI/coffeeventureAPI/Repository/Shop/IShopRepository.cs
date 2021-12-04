using ShopEntity = coffeeventureAPI.Data.Shop;
using ShopCategoryEntity = coffeeventureAPI.Data.ShopCategory;
using ImageEntity = coffeeventureAPI.Data.Image;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using coffeeventureAPI.Model;
using coffeeventureAPI.Data;
using CategoryEntity = coffeeventureAPI.Data.Category;
using Microsoft.AspNetCore.Http;

namespace coffeeventureAPI.Repository.Shop
{
    public interface IShopRepository : IScoped
    {
        Task<ShopDto> SelectById(string id);
        Task<IEnumerable<ShopDto>> Select(ShopRequestDto request);
        Task<IEnumerable<ShopDto>> View(ShopRequestDto request);
        Task<int> Count(ShopRequestDto request);
        Task<ShopDto> Merge(ShopDto model);
        Task<bool> BulkMergeShop(IEnumerable<ShopCategoryEntity> CategoryShops, string ShopId);
        Task<bool> BulkMergeShopCategory(IEnumerable<ShopCategoryEntity> Categories, string ShopId);
        Task<bool> DeleteById(params string[] ids);
        Task<List<CategoryEntity>> GetShopCategory(CategoryRequestDto request);
        Task<int> CountShopCategory(CategoryRequestDto request);
       Task<CategoryDto> MergeShopCategory(CategoryDto requestDto);
        Task<bool> DeleteShopCategory(string id);
        Task<ImageEntity> Upload(IFormFile file, string shopId);
        Task<List<ShopEntity>> SelectShopByCategory(string CategoryId);
        Task<ShopSearchDto> SearchShop();
    }
}
