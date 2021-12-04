using coffeeventureAPI.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace coffeeventureAPI.Data.UserDto
{
    public class UserShopRequestDto : BaseRequestDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string[] ShopIds { get; set; }
        public string ShopId { get; set; }
    }
}
