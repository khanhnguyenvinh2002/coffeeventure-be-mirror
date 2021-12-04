
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ShopEntity = coffeeventureAPI.Data.Shop;
using ReviewEntity = coffeeventureAPI.Data.Review;
using ReviewImageEntity = coffeeventureAPI.Data.ReviewImage;
using ReviewLikeEntity = coffeeventureAPI.Data.ReviewLike;
using ShopCategoryEntity = coffeeventureAPI.Data.ShopCategory;
using CategoryEntity = coffeeventureAPI.Data.Category;
using ImageEntity = coffeeventureAPI.Data.Image;
using UserShopEntity = coffeeventureAPI.Data.UserShop;
using ShopeImageEntity = coffeeventureAPI.Data.ShopImage;

using coffeeventureAPI.Data;
using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Model;
using System.Net.Http.Headers;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Logging;
using coffeeventureAPI.Service.Blob;

namespace coffeeventureAPI.Repository.Shop
{
    public class ShopRepository : BaseRepository, IShopRepository
    {
        private string _rootPath = string.Empty;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private string connectionString;
        private IBlobService _blobService;
        private CVDbContext _context;

        private readonly ILogger<ShopRepository> _logger;
        #region Constructor

        public ShopRepository(IUnitOfWork unitOfWork, ILogger<ShopRepository> logger, IConfiguration configuration, CVDbContext context, IBlobService blobService)
        {
            _blobService = blobService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _rootPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.ToString() + _configuration["UploadLocation:MainPath"];
            connectionString = _configuration["ConnectionStrings:DefaultConnection"];
        }

        #endregion

        public async Task<ShopDto> SelectById(string id)
        {
            List<string> temp = new List<string>();
            IQueryable<ShopEntity> query = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            var ishop = _unitOfWork.Select<ShopeImageEntity>().AsNoTracking();
            var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            var shopCategory = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking();
            var category = _unitOfWork.Select<CategoryEntity>().AsNoTracking();
            var shop = query
                .Where(x => x.Id == id)
                .Select(x => new ShopDto(x)).SingleOrDefault();
            var imageIds = ishop.Where(x => x.ShopId == shop.Id).Select(x => x.ImageId).ToList();
            var shopCate = shopCategory.Where(x => x.ShopId == shop.Id).Select(x => x.CategoryId);
            var cate = category.Where(x => shopCate.Contains(x.Id)).Select(x => new CategoryDtox(x) { });
            shop.ShopCategory = cate;            
            shop.NumSaved = _unitOfWork.Select<UserShopEntity>().AsNoTracking().Where((x => x.ShopId == shop.Id)).ToList().Count();

            foreach (var imageId in imageIds)
            {
                if (!string.IsNullOrEmpty(imageId))
                    temp.Add(images.Where(x => x.Id == imageId).Select(x => x.Path).FirstOrDefault());
            }
            shop.ImageDirectories = temp; 
            var reviewShop = _unitOfWork.Select<ReviewEntity>().Where(x => x.ShopId == id ).ToList().Select(x => x.Rating).DefaultIfEmpty(0).Average();;
            shop.Rating = reviewShop;   
            return await Task.FromResult(shop);
        }

        public async Task<IEnumerable<ShopDto>> Select(ShopRequestDto request)
        {
            IQueryable<ShopEntity> query = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            var userShop = _unitOfWork.Select<UserShopEntity>().AsNoTracking();
            var ishop = _unitOfWork.Select<ShopeImageEntity>().AsNoTracking();
            var shopCategory = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking();
            var category = _unitOfWork.Select<CategoryEntity>().AsNoTracking();
            var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            query = Filter(query, request);
            query = query.Paging(request);
            var ans = query.Select(x => new ShopDto(x)).ToList();
            foreach (var i in ans)
            {
                var imageId = ishop.Where(x => x.ShopId == i.Id).Select(x => x.ImageId).FirstOrDefault();
                var shopCate = shopCategory.Where(x => x.ShopId == i.Id).Select(x=> x.CategoryId);
                var cate = category.Where(x => shopCate.Contains(x.Id)).Select(x => new CategoryDtox(x) { });
                i.ShopCategory = cate;
                var reviewShop = _unitOfWork.Select<ReviewEntity>().Where(x => x.ShopId == i.Id ).ToList().Select(x => x.Rating).DefaultIfEmpty(0).Average();;
                i.Rating = reviewShop;
                i.NumSaved = userShop.Where((x => x.ShopId == i.Id)).ToList().Count();
                if (!string.IsNullOrEmpty(imageId))
                    i.ImagePath = images.Where(x => x.Id == imageId).Select(x => x.Path).FirstOrDefault();
            }
            return await Task.FromResult(ans);
        }
        public async Task<IEnumerable<ShopDto>> View(ShopRequestDto request)
        {
            var query = _unitOfWork.Select<ShopEntity>().Select(x=>new ShopDto(x)).AsNoTracking();
           
            return await Task.FromResult(query.ToList());
        }
        public async Task<int> Count(ShopRequestDto request)
        {
            IQueryable<ShopEntity> query = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }

        public async Task<ShopDto> Merge(ShopDto model)
        {
          
            var result = _unitOfWork.Merge<ShopEntity, ShopDto>(model);
            return await Task.FromResult(model);
        }

        public async Task<bool> BulkMergeShop(IEnumerable<ShopCategoryEntity> shopCategories, string ShopId)
        {
            var itemDelete = _unitOfWork.Select<ShopCategoryEntity>().Where(x => x.ShopId == ShopId);
            //_unitOfWork.BulkMerge(shopCategories);
            _unitOfWork.BulkDelete(itemDelete);
            _unitOfWork.BulkInsert(shopCategories);
            return await Task.FromResult(true);
        }

        public async Task<bool> BulkMergeShopCategory(IEnumerable<ShopCategoryEntity> Categories, string ShopId)
        {
            var itemDelete = _unitOfWork.Select<ShopCategoryEntity>().Where(x => x.ShopId == ShopId);
            _unitOfWork.BulkDelete(itemDelete);
            _unitOfWork.BulkInsert(Categories);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteById(params string[] ids)
        {
            var reviewShop = _unitOfWork.Select<ReviewEntity>().Where(x => ids.Contains(x.ShopId));
            var reviewIds = reviewShop.Select(x => x.Id).ToList();
            var reviewImageShop = _unitOfWork.Select<ReviewImageEntity>().Where(x => reviewIds.Contains(x.ReviewId));
            var reviewLikeShop = _unitOfWork.Select<ReviewLikeEntity>().Where(x => reviewIds.Contains(x.ReviewId));
            var userShop = _unitOfWork.Select<UserShopEntity>().Where(x => ids.Contains(x.ShopId));
            var journalShop = _unitOfWork.Select<JournalShop>().Where(x => ids.Contains(x.ShopId));
            var shopImage = _unitOfWork.Select<ShopeImageEntity>().Where(x => ids.Contains(x.ShopId));
            var shopCategoryDelete = _unitOfWork.Select<ShopCategoryEntity>().Where(x => ids.Contains(x.ShopId));
            _unitOfWork.BulkDelete(reviewImageShop);
            _unitOfWork.BulkDelete(reviewLikeShop);
            _unitOfWork.BulkDelete(reviewShop);
            _unitOfWork.BulkDelete(userShop);
            _unitOfWork.BulkDelete(journalShop);
            _unitOfWork.BulkDelete(shopImage);
            _unitOfWork.BulkDelete(shopCategoryDelete);
            var shopDelete = _unitOfWork.Select<ShopEntity>().Where(x => ids.Contains(x.Id));
            _unitOfWork.BulkDelete(shopDelete);
            return await Task.FromResult(true);
        }

        public async Task<List<CategoryEntity>> GetShopCategory(CategoryRequestDto request)
        {
            IQueryable<CategoryEntity> query = _unitOfWork.Select<CategoryEntity>().AsNoTracking();
            query = FilterCategory(query, request).OrderBy(x => x.Name);
            query = query.Paging(request);
            return await Task.FromResult(query.ToList());

        }
        public async Task<int> CountShopCategory(CategoryRequestDto request)
        { 

            IQueryable<CategoryEntity> query = _unitOfWork.Select<CategoryEntity>().AsNoTracking();
            query = FilterCategory(query, request);
            return await query.CountAsync();

        }
        public async Task<CategoryDto> MergeShopCategory(CategoryDto requestDto)
        {
            var result = _unitOfWork.Merge<CategoryEntity, CategoryDto>(requestDto);
            return await Task.FromResult(result);

        }
        public async Task<bool> DeleteShopCategory(string id)
        {
            var ShopCategoryDelete = _unitOfWork.Select<ShopCategoryEntity>().Where(x => x.CategoryId == id);
            _unitOfWork.BulkDelete(ShopCategoryDelete);
            var categoryDelete = _unitOfWork.Select<CategoryEntity>().Where(x => x.Id == id);
            _unitOfWork.BulkDelete(categoryDelete);
            return await Task.FromResult(true);

        }
        public async Task<ImageEntity> Upload(IFormFile file,string shopId)
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
                var fileImage = new ShopeImageEntity() { Id = Guid.NewGuid().ToString("N"), ImageId = entity.Id, ShopId = shopId };
                _unitOfWork.Insert(entity);
                _unitOfWork.Insert(fileImage);

            
            }

            return await Task.FromResult(entity);
        }
        public async Task<ShopSearchDto> SearchShop()
        {
            var query = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            var categories = _unitOfWork.Select<CategoryEntity>().AsNoTracking().Select(x => new CategoryDtox { CategoryId = x.Id, Name = x.Name }).ToList();
            var districts = query.Select(x=> new DistrictDto { Name = x.District, District = x.District, City = x.City }).Distinct().ToList();
            var streets = query.Select(x => new StreetDto { Name = x.Street, Street = x.Street, District = x.District, City = x.City }).Distinct().ToList();
            var cities = query.Select(x => x.City).Distinct().Select(x => new CityDto { Name = x, City = x }).ToList();
            return await Task.FromResult(new ShopSearchDto() { Categories = categories, Districts = districts, Cities = cities, Streets = streets });
        }
        public async Task<List<ShopEntity>> SelectShopByCategory(string categoryId)
        {
            var ShopIds = _unitOfWork.Select<ShopCategoryEntity>().Where(x => x.CategoryId == categoryId)
                .Select(x => x.ShopId).Distinct();

            IQueryable<ShopEntity> query = _unitOfWork.Select<ShopEntity>().AsNoTracking();
            var listShop = new List<ShopEntity>();
            if (ShopIds != null)
            {
                foreach (var item in ShopIds)
                {
                    var Shop = query.Where(x => x.Id == item).SingleOrDefault();
                    if (Shop != null)
                    {
                        listShop.Add(Shop);
                    }
                }
            }
            return await Task.FromResult(listShop);
        }
        //private IQueryable<ShopDto> FilterDto(IQueryable<ShopDto> models, ShopRequestDto searchEntity)
        //{
        //    var ids = models.Select(x => x.Id).ToList();
        //    var categoryIds = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking().Where(x => ids.Contains(x.ShopId)).Select(x=> x.CategoryId);
        //    if (searchEntity.CategoryIds != null && searchEntity.CategoryIds.Length > 0)
        //    {
        //        var temp = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking().Where(x => searchEntity.CategoryIds.Contains(x.CategoryId)).Select(x => x.ShopId);
        //        models = models.Where(x => temp.Contains(x.Id));
        //    }
        //    if (searchEntity.Districts != null && searchEntity.Districts.Length > 0)
        //    {
        //        models = models.Where(x => searchEntity.Districts.Contains(x.District));
        //    }
        //    if (searchEntity.Cities != null && searchEntity.Cities.Length > 0)
        //    {
        //        models = models.Where(x => searchEntity.Cities.Contains(x.City));
        //    }
        //    if (!string.IsNullOrEmpty(searchEntity.Id))
        //    {
        //        models = models.Where(x => x.Id == searchEntity.Id);
        //    }

        //    if (!string.IsNullOrEmpty(searchEntity.Description))
        //    {
        //        models = models.Where(x => x.Description == searchEntity.Description);
        //    }
        //    if (!string.IsNullOrEmpty(searchEntity.Address))
        //    {
        //        models = models.Where(x => x.Address == searchEntity.Address);
        //    }
        //    if (!string.IsNullOrEmpty(searchEntity.District))
        //    {
        //        models = models.Where(x => x.District == searchEntity.District);
        //    }
        //    if (!string.IsNullOrEmpty(searchEntity.City))
        //    {
        //        models = models.Where(x => x.City == searchEntity.City);
        //    }
        //    if (!string.IsNullOrEmpty(searchEntity.Street))
        //    {
        //        models = models.Where(x => x.Street == searchEntity.Street);
        //    }
        //    if (searchEntity.MinPrice > 0)
        //    {
        //        models = models.Where(x => x.MinPrice == searchEntity.MinPrice);
        //    }
        //    if (searchEntity.MinPrice > 0)
        //    {
        //        models = models.Where(x => x.MaxPrice == searchEntity.MaxPrice);
        //    }
        //    if (!string.IsNullOrEmpty(searchEntity.Name))
        //    {
        //        models = models.Where(x => x.Name.Contains(searchEntity.Name));
        //    }

        //    return models;
        //}
        #region Private methods

        private IQueryable<ShopEntity> Filter(IQueryable<ShopEntity> models, ShopRequestDto searchEntity)
        {
            var ids = models.Select(x => x.Id).ToList();
            var categoryIds = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking().Where(x => ids.Contains(x.ShopId)).Select(x => x.CategoryId);
            if (searchEntity.CategoryIds != null && searchEntity.CategoryIds.Length > 0)
            {
                var temp = _unitOfWork.Select<ShopCategoryEntity>().AsNoTracking().Where(x => searchEntity.CategoryIds.Contains(x.CategoryId)).Select(x => x.ShopId);
                models = models.Where(x => temp.Contains(x.Id));
            }
            if (searchEntity.ExcludeIds != null && searchEntity.ExcludeIds.Length > 0)
            {
                models = models.Where(x => !searchEntity.ExcludeIds.Contains(x.Id));
            }
                if (searchEntity.Districts != null && searchEntity.Districts.Length > 0)
            {
                models = models.Where(x => searchEntity.Districts.Contains(x.District));
            }
            if (searchEntity.Cities != null && searchEntity.Cities.Length > 0)
            {
                models = models.Where(x => searchEntity.Cities.Contains(x.City));
            }
            if (searchEntity.Streets != null && searchEntity.Streets.Length > 0)
            {
                models = models.Where(x => searchEntity.Streets.Contains(x.Street));
            }
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.Id == searchEntity.Id);
            }
            if (!string.IsNullOrEmpty(searchEntity.OpeningHour))
            {
                models = models.Where(x => x.OpeningHour == searchEntity.OpeningHour);
            }
            if (!string.IsNullOrEmpty(searchEntity.EndingHour))
            {
                models = models.Where(x => x.EndingHour == searchEntity.EndingHour);
            }

            if (!string.IsNullOrEmpty(searchEntity.Description))
            {
                models = models.Where(x => x.Description == searchEntity.Description);
            }
            if (!string.IsNullOrEmpty(searchEntity.Address))
            {
                models = models.Where(x => x.Address == searchEntity.Address);
            }
            if (!string.IsNullOrEmpty(searchEntity.District))
            {
                models = models.Where(x => x.District == searchEntity.District);
            }
            if (!string.IsNullOrEmpty(searchEntity.City))
            {
                models = models.Where(x => x.City == searchEntity.City);
            }
            if (!string.IsNullOrEmpty(searchEntity.Street))
            {
                models = models.Where(x => x.Street == searchEntity.Street);
            }
            if (searchEntity.MinPrice > 0)
            {
                models = models.Where(x => x.MinPrice == searchEntity.MinPrice);
            }
            if (searchEntity.MinPrice > 0)
            {
                models = models.Where(x => x.MaxPrice == searchEntity.MaxPrice);
            }
            if (!string.IsNullOrEmpty(searchEntity.Name))
            {
                models = models.Where(x => x.Name.Contains(searchEntity.Name));
            }

            return models;
        }
        private IQueryable<CategoryEntity> FilterCategory(IQueryable<CategoryEntity> models, CategoryRequestDto searchEntity)
        {
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.Id == searchEntity.Id);
            }

            if (!string.IsNullOrEmpty(searchEntity.Name))
            {
                models = models.Where(x => x.Name == searchEntity.Name);
            }
            if (!string.IsNullOrEmpty(searchEntity.Code))
            {
                models = models.Where(x => x.Code == searchEntity.Code);
            }
            return models;
        }

        #endregion Private methods
    }
}