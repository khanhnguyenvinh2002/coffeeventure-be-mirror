
using System.Collections.Generic;
using CategoryEntity = coffeeventureAPI.Data.Category;
using coffeeventureAPI.Core.Base.Model;

namespace coffeeventureAPI.Data
{
    public class CategoryDto : BaseModel
    {
        public CategoryDto()
        {
        }

        public CategoryDto(CategoryEntity entity) : base(entity)
        {
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public IEnumerable<CategoryDtox> ShopCategory { get; set; }
    }

}
