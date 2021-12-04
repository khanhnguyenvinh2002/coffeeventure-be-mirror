using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Data
{
    public partial class Shop { 
        public Shop()
        {
            ShopCategory = new HashSet<ShopCategory>();
            ShopImage = new HashSet<ShopImage>();
            JournalShop = new HashSet<JournalShop>();
            UserShop = new HashSet<UserShop>();
        }

        public virtual ICollection<UserShop> UserShop { get; set; }
        public virtual ICollection<JournalShop> JournalShop { get; set; }
        public virtual ICollection<ShopImage> ShopImage { get; set; }
        public string Id { get; set; }
        public string Price { get; set; }
        public string Telephone { get; set; }
        public string OpeningHour { get; set; }
        public string EndingHour { get; set; }
        public string ALternativeTelephone { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int? Status { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual ICollection<ShopCategory> ShopCategory { get; set; }
    }
}
