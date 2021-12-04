
using System.Collections.Generic;
using OperationEntity = coffeeventureAPI.Data.Operation;
using coffeeventureAPI.Core.Base.Model;

namespace coffeeventureAPI.Data
{
    public class OperationDto : BaseModel
    {
        public OperationDto()
        {
        }

        public OperationDto(OperationEntity entity) : base(entity)
        {
            if (entity.ParentMenuNavigation != null)
            {
                ParentOperation = new ParentMenuNavigationDto()
                {
                    Id = entity.ParentMenuNavigation.Id,
                    Name = entity.ParentMenuNavigation.Name
                };
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public string Link { get; set; }
        public bool Type { get; set; }
        public string ParentMenu { get; set; }
        public int MenuOrder { get; set; }
        public string MenuIcon { get; set; }
        public string[] ActionId { get; set; }
        public ParentMenuNavigationDto ParentOperation { get; set; }
        public IEnumerable<ActionDtox> User { get; set; }
    }

    public class ParentMenuNavigationDto : BaseModel
    {
        public ParentMenuNavigationDto()
        {
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class ActionDtox : BaseModel
    {
        public ActionDtox()
        {
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
