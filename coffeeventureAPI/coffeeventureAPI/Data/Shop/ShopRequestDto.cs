
using coffeeventureAPI.Dtos.Base;

namespace coffeeventureAPI.Data
{
    public class ShopRequestDto : BaseRequestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Telephone { get; set; }
        public string ALternativeTelephone { get; set; }
        public string Category { get; set; }
        public string[] ExcludeIds { get; set; }
        public string[] CategoryIds { get; set; }
        public string[] Streets { get; set; }
        public string[] Districts { get; set; }
        public string[] Cities { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int MaxPrice { get; set; }
        public int MinPrice { get; set; }
        
        public double? Latitude { get; set; }
        
        public double? Longitude { get; set; }
        public string OpeningHour { get; set; }
        public string EndingHour { get; set; }

    }
}
