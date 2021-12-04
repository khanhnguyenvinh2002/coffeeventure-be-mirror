
using coffeeventureAPI.Core.Base.Model;
using coffeeventureAPI.Dtos.Base;
using System.Collections.Generic;
using CategoryEntity = coffeeventureAPI.Data.Category;

namespace coffeeventureAPI.Data
{
    public class CategoryRequestDto : BaseRequestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public IEnumerable<CategoryDtox> ShopCategory { get; set; }
    }

    public class CategoryDtox : BaseModel
    {
        public CategoryDtox()
        {
        }

        public CategoryDtox(CategoryEntity x): base(x)
        {
        }
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string Name { get; set; }
    }
}


