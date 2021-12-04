using System;
using System.Linq;
using System.Threading.Tasks;
using ShopCategoryEntity = coffeeventureAPI.Data.ShopCategory;
using ShopModel = coffeeventureAPI.Data.Shop;
using ShopImageEntity = coffeeventureAPI.Data.ShopImage;
using ImageEntity = coffeeventureAPI.Data.Image;
using System.Collections.Generic;
using coffeeventureAPI.Data;
using coffeeventureAPI.Core.Service;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Repository.Shop;
using System.Data.Entity;
using CategoryEntity = coffeeventureAPI.Data.Category;
using Microsoft.AspNetCore.Http;

namespace coffeeventureAPI.Service
{
    public class ShopService : BaseService, IShopService
    {
        #region Private variables

        private readonly IShopRepository _shopRepository;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private variables

        public ShopService(IShopRepository ShopRepository, IUnitOfWork unitOfWork) : base()
        {
            _shopRepository = ShopRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ShopDto> SelectById(string id)
        {
            return await _shopRepository.SelectById(id);
        }

        public async Task<IEnumerable<ShopDto>> Select(ShopRequestDto request)
        {
            return await _shopRepository.Select(request);
        }

        public async Task<IEnumerable<ShopDto>> View(ShopRequestDto request)
        {
            return await _shopRepository.View(request);
        }
        public async Task<int> Count(ShopRequestDto request)
        {
            return await _shopRepository.Count(request);
        }
        public async Task<ShopDto> Merge(ShopDto dto)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();

            // Merge Shop
            var dtoResult = await _shopRepository.Merge(dto);

            // Merge Shop Category
            var shopCategories = dto.ShopCategory?.Select(x => new ShopCategoryEntity() { Id = Guid.NewGuid().ToString("N"), ShopId = dtoResult.Id, CategoryId = x.Id }).ToList();
            if (dto.Categories != null && dto.Categories.Length > 0)
            {
                foreach (var i in dto.Categories)
                {
                    shopCategories.Add(new ShopCategoryEntity() { Id = Guid.NewGuid().ToString("N"), ShopId = dtoResult.Id, CategoryId = i });
                }
            }
            if (shopCategories != null && shopCategories.Count() > 0)
            {
                await _shopRepository.BulkMergeShop(shopCategories, dtoResult.Id);
            }
            // Commit transaction
            transaction.Commit();
            return await _shopRepository.SelectById(dto.Id);
        }
        //public async Task<ShopDto> Merge(ShopDto dto, IFormFileCollection files)
        //{
        //    // Begin transaction
        //    using var transaction = _unitOfWork.BeginTransaction();

        //    // Merge Shop
        //    var dtoResult = await _shopRepository.Merge(dto);

        //    // Merge Shop Category
        //    var shopCategories = dto.ShopCategory?.Select(x => new ShopCategoryEntity() { Id = Guid.NewGuid().ToString("N"), ShopId = dtoResult.Id, CategoryId = x.Id }).ToList();
        //    if (dto.Categories != null && dto.Categories.Length > 0)
        //    {
        //        foreach(var i in dto.Categories)
        //        {
        //            shopCategories.Add(new ShopCategoryEntity() { Id = Guid.NewGuid().ToString("N"), ShopId = dtoResult.Id, CategoryId = i });
        //        }
        //    }
        //    if (shopCategories != null && shopCategories.Count() > 0)
        //    {
        //        await _shopRepository.BulkMergeShop(shopCategories, dtoResult.Id);
        //    }
        //    await Upload(files, dtoResult.Id);
        //    // Commit transaction
        //    transaction.Commit();
        //    return await _shopRepository.SelectById(dto.Id);
        //}

        public async Task<bool> BulkMergeShopCategory(IEnumerable<string> categories, string ShopId)
        {
            var ShopOperations = new List<ShopCategoryEntity>();
            foreach (var category in categories)
            {
                ShopOperations.Add(new ShopCategoryEntity() { Id = Guid.NewGuid().ToString("N"), ShopId = ShopId, CategoryId = category });
            }
            return await _shopRepository.BulkMergeShopCategory(ShopOperations, ShopId);
        }

        public async Task<string[]> SelectShopCategory(string ShopId)
        {
            var result = _unitOfWork.Select<ShopCategoryEntity>().Where(x => x.ShopId == ShopId).Select(x => x.CategoryId).ToArray();
            return await Task.FromResult(result);
        }

        public async Task<List<ShopCategoryEntity>> GetShopCategory()
        {
            var result = _unitOfWork.Select<ShopCategoryEntity>().ToList();
            return await Task.FromResult(result);
        }
        public async Task<int> CountShopCategory()
        {
            IQueryable<ShopCategoryEntity> query = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking();
            return await query.CountAsync();
        }
        public async Task<List<CategoryEntity>> GetShopCategory(CategoryRequestDto request)
        {
            return await _shopRepository.GetShopCategory(request);
        }

        public async Task<int> CountShopCategory(CategoryRequestDto request)
        {
            return await _shopRepository.CountShopCategory(request);
        }
        public async Task<CategoryDto> MergeShopCategory(CategoryDto requestDto)
        {
            return await _shopRepository.MergeShopCategory(requestDto);
        }
        public async Task<bool> DeleteShopCategory(string id)
        {
            return await _shopRepository.DeleteShopCategory(id);
        }
        public async Task<List<ImageEntity>> Upload(IFormFileCollection files, string shopId)
        {

            List<ImageEntity> result = new List<ImageEntity>();

            var currImg = _unitOfWork.Select<ShopImageEntity>().Where(x => x.ShopId == shopId).AsNoTracking();
            _unitOfWork.BulkDelete(currImg);
            foreach (var file in files)
            {
                ImageEntity fileInfo = await _shopRepository.Upload(file, shopId);
                result.Add(fileInfo);
            }
            return result;
        }
        public async Task<bool> Delete(string id)
        {
            using var transaction = _unitOfWork.BeginTransaction();
            var result = await _shopRepository.DeleteById(id);
            transaction.Commit();
            return result;
        }
        public async Task<ShopSearchDto> SearchShop()
        {
            return await _shopRepository.SearchShop();
        }
    }
}
