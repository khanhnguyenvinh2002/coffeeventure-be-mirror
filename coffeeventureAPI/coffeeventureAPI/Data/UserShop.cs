using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class UserShop
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ShopId { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual User User { get; set; }
    }
}
