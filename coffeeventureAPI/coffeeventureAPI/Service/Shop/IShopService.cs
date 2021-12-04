using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopModel = coffeeventureAPI.Data.Shop;
using coffeeventureAPI.Model;
using ImageEntity = coffeeventureAPI.Data.Image;
using coffeeventureAPI.Data;
using ShopCategoryEntity = coffeeventureAPI.Data.ShopCategory;
using CategoryEntity = coffeeventureAPI.Data.Category;
using Microsoft.AspNetCore.Http;

namespace coffeeventureAPI.Service
{
    public interface IShopService : IScoped
    {
        Task<ShopDto> SelectById(string id);
        Task<IEnumerable<ShopDto>> Select(ShopRequestDto request);
        Task<IEnumerable<ShopDto>> View(ShopRequestDto request);
        Task<int> Count(ShopRequestDto request);
        Task<ShopDto> Merge(ShopDto dto);
        //Task<ShopDto> Merge(ShopDto dto, IFormFileCollection files);
        Task<bool> BulkMergeShopCategory(IEnumerable<string> CategoryIds, string ShopId);
        Task<string[]> SelectShopCategory(string ShopId);
        Task<List<CategoryEntity>> GetShopCategory(CategoryRequestDto request);
        Task<int> CountShopCategory(CategoryRequestDto request);
        Task<CategoryDto> MergeShopCategory(CategoryDto requestDto);
        Task<bool> DeleteShopCategory( string id);
        Task<List<ImageEntity>> Upload(IFormFileCollection files, string shopId);
        Task<bool> Delete(string id);
        Task<ShopSearchDto> SearchShop();
        //Task<IEnumerable<CategoryEntity>> SelectMenuCategory(string ShopId);
        //Task<IEnumerable<CategoryEntity>> SelectMenuCategoryCustomized(string ShopId);
    }
}
