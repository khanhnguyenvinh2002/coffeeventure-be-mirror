
using System.Collections.Generic;
using OperationEntity = coffeeventureAPI.Data.Operation;
using coffeeventureAPI.Core.Base.Model;

namespace coffeeventureAPI.Data
{
    public class MenuDto : BaseModel
    {
        public MenuDto(OperationEntity entity) {
            Id = entity.Id;
            Title = entity.Name;
            Root = string.IsNullOrEmpty(entity.ParentMenu);
            Alignment = Root ? "left" : null;
            Page = entity.Link;
            Translate = entity.Name;
            Icon = entity.MenuIcon;
            Index = entity.MenuOrder;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public bool Root { get; set; }
        public string Alignment { get; set; }
        public string Page { get; set; }
        public string Translate { get; set; }
        public string Icon { get; set; }
        public int Index { get; set; }
        public List<MenuDto> Submenu { get; set; }
    }
}
