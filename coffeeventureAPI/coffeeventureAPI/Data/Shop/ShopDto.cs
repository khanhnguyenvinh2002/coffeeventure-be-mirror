
using System.Collections.Generic;
using ShopEntity = coffeeventureAPI.Data.Shop;
using System.Linq;
using coffeeventureAPI.Core.Base.Model;
using System;

namespace coffeeventureAPI.Data
{
    public class ShopDto : BaseModel
    {
        public ShopDto()
        {
        }
        public ShopDto(ShopEntity entity) : base(entity)
        {
        }

        public ShopDto(ShopEntity entity, string ImagePath) : base(entity)
        {
            this.ImagePath = ImagePath;
        }
        public string Id { get; set; }
        public string Price { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string OpeningHour { get; set; }
        public string EndingHour { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string ALternativeTelephone { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int NumSaved { get; set; }
        public int? Status { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public double? Rating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string[] Categories { get; set; }
        public string ImagePath { get; set; }
        public IEnumerable<string> ImageDirectories { get; set; }

        public IEnumerable<CategoryDtox> ShopCategory { get; set; }
    }

}
