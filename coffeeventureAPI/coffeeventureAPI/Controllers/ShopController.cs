using coffeeventureAPI.Data;
using coffeeventureAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
//using CategoryEntity = coffeeventureAPI.Data.Category;
using ShopCategoryEntity = coffeeventureAPI.Data.ShopCategory;
using CategoryEntity = coffeeventureAPI.Data.Category;
using ImageEntity = coffeeventureAPI.Data.Image;
using System.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace coffeeventureAPI.Controllers
{
    [Route("shop")]
    [Authorize]
    public class ShopController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShopService _shopService;
        public ShopController(IShopService ShopService, IHttpContextAccessor httpContextAccessor)
        {
            _shopService = ShopService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ShopDto> SelectById([FromRoute] string id)
        {
            return await _shopService.SelectById(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ShopDto>> Select([FromQuery] ShopRequestDto request)
        {
            return await _shopService.Select(request);
        }

        [Route("view")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ShopDto>> View([FromQuery] ShopRequestDto request)
        {
            return await _shopService.View(request);
        }
        [Route("count")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<int> Count([FromQuery] ShopRequestDto request)
        {
            return await _shopService.Count(request);
        }
        //[FromBody] ShopDto requestDto
        [Route("merge")]
        [HttpPost]
        public async Task<ShopDto> Merge ([FromBody] ShopDto dto)
        {
            return await _shopService.Merge(dto);
        }
        //public async Task<ShopDto> Merge([FromForm] JObject juser)
        //{
        //    var file = Request.Form.Files;
        //    var stream = Request.Headers["body"];
        //    string json = JsonConvert.SerializeObject(stream);
        //    //JObject juser = JObject.Parse(stream);
        //    JToken shop = juser["shopCategory"];
        //    IEnumerable<CategoryDtox> obj = null;
        //    ShopDto requestDto = new ShopDto();
        //    if (shop != null)
        //    {requestDto.ShopCategory= shop.ToObject<IEnumerable<CategoryDtox>>();
        //    }
        //    //IEnumerable<CategoryDtox> temp = new CategoryDtox() { CategoryId = (string)shop["id"] };
        //    //JToken juser = jobject["shopCategory"];
        //    if(juser["id"] != null)
        //    {
        //        requestDto.Id = (string)juser["id"];
        //    }
        //    if (juser["name"] != null)
        //    {
        //        requestDto.Name = (string)juser["name"];
        //    }
        //    if (juser["price"] != null)
        //    {
        //        requestDto.Price = (string)juser["price"];
        //    }
        //    if (juser["category"] != null)
        //    {
        //        requestDto.Category = (string)juser["category"];
        //    }
        //    if (juser["description"] != null)
        //    {
        //        requestDto.Description = (string)juser["description"];
        //    }
        //    if (juser["address"] != null)
        //    {
        //        requestDto.Address = (string)juser["address"];
        //    }
        //    if (juser["district"] != null)
        //    {
        //        requestDto.Id = (string)juser["district"];
        //    }
        //    if (juser["city"] != null)
        //    {
        //        requestDto.City = (string)juser["city"];
        //    }
        //    if (juser["street"] != null)
        //    {
        //        requestDto.Street = (string)juser["street"];
        //    }
        //    if (juser["maxPrice"] != null)
        //    {
        //        requestDto.MaxPrice = (int)juser["maxPrice"];
        //    }
        //    if (juser["minPrice"] != null)
        //    {
        //        requestDto.MinPrice = (int)juser["minPrice"];
        //    }

        //    return await _shopService.Merge(requestDto,file );
        //}
        [Route("merge-shop-category/{ShopId}")]
        [HttpPost]
        public async Task<bool> BulkMergeShopCategory([FromBody] IEnumerable<string> Categories, [FromRoute] string ShopId)
        {
            return await _shopService.BulkMergeShopCategory(Categories, ShopId);
        }

        [Route("select-shop-category/{ShopId}")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<string[]> SelectShopCategory([FromRoute] string ShopId)
        {
            return await _shopService.SelectShopCategory(ShopId);
        }

        [Route("select-category")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<List<CategoryEntity>> GetCategory([FromQuery] CategoryRequestDto request)
        {
            return await _shopService.GetShopCategory(request);
        }

        [Route("count-category")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<int> CountCategory([FromQuery] CategoryRequestDto request)
        {
            return await _shopService.CountShopCategory(request);
        }
        [Route("merge-category")]
        [HttpPost]
        public async Task<CategoryDto> MergeCategory([FromBody] CategoryDto requestDto)
        {
            return await _shopService.MergeShopCategory(requestDto);
        }
        [Route("delete-category/{id}")]
        [HttpDelete]
        public async Task<bool> DeleteCategory([FromRoute] string id)
        {
            return await _shopService.DeleteShopCategory(id);
        }
        [Route("upload-images/{shopId}")]
        [HttpPost]
        public async Task<List<ImageEntity>> Upload([FromRoute] string shopId)
        {
            //if (Request.Headers["id-token"].Count() > 0)
            //{
            //    var stream = Request.Headers["id-token"];
            //    var handler = new JwtSecurityTokenHandler();
            //    var jsonToken = handler.ReadToken(stream);
            //    var tokenS = jsonToken as JwtSecurityToken;
            //    var jti = tokenS.Claims.First(claim => claim.Type == "jti").Value;
            //    _unitOfWork.SetCurrentUserId(jti);
            //}
            return await _shopService.Upload(Request.Form.Files, shopId);
        }
        [Route("{id}")]
        [HttpDelete]
        public async Task<bool> Delete([FromRoute] string id)
        {
            return await _shopService.Delete(id);
        }

        [Route("select-shop-search")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ShopSearchDto> SearchShop()
        {
            return await _shopService.SearchShop();
        }

    }
}
