using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Data
{
    public class ShopSearchDto
    {
        public List<CategoryDtox> Categories { get; set; }
        public List<DistrictDto> Districts { get; set; }
        public List<CityDto> Cities { get; set; }
        public List<StreetDto> Streets { get; set; }
    }
    public class DistrictDto
    {
        public string Name { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }

    public class StreetDto
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }
    public class CityDto
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
}
