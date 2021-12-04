using coffeeventureAPI.Data;
using coffeeventureAPI.Data.UserDto;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopEntity = coffeeventureAPI.Data.Shop;

namespace coffeeventureAPI.Controllers
{
    [Route("user-shop")]
    [Authorize]
    public class UserShopController : Controller
    {
        private readonly IUserShopService _userShopService;
        private readonly IUnitOfWork _unitOfWork;
        public UserShopController(IUserShopService userShopService, IUnitOfWork unitOfWork)
        {
            _userShopService = userShopService;
            _unitOfWork = unitOfWork;
        }

        [Route("count")]
        [HttpGet]
        public async Task<int> CountShop([FromQuery] UserShopRequestDto request)
        {
            return await _userShopService.Count(request);
        }

        [HttpGet]
        public async Task<IEnumerable<ShopDto>> SelectShop([FromQuery] UserShopRequestDto request)
        {
            return await _userShopService.Select(request);
        }

        [Route("get-all")]
        [HttpGet]
        public async Task<IEnumerable<ShopDto>> GetAll()
        {
            return await _userShopService.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> SelectShopById([FromRoute] string id)
        {
            var result = await _userShopService.SelectById(id);
            return Ok(result);
        }
        [Route("bulk-insert")]
        [HttpPost]
        public async Task<bool> BulkInsertShop([FromBody] UserShopRequestDto userShops)
        {
            return await _userShopService.BulkInsertShop(userShops.ShopIds, userShops.ShopId);
        }

        [HttpPost]
        public async Task<bool> InsertShop([FromBody] UserShopRequestDto userShop)
        {
            return await _userShopService.InsertShop(_unitOfWork.GetCurrentUserId(), userShop.Id);
        }
        [Route("delete-user-shop")]
        [HttpDelete]
        public async Task<bool> Delete([FromQuery] UserShopRequestDto request)
        {
            return await _userShopService.Delete(request);
        }
    }
}
