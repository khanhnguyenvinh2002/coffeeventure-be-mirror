using coffeeventureAPI.Core.Base.Model;

using ActionEntity = coffeeventureAPI.Data.Action;
namespace coffeeventureAPI.Data
{
    public class ActionDto : BaseModel
    {
        public ActionDto()
        {
        }

        public ActionDto(ActionEntity entity) : base(entity)
        {
        }

        public string Id { get; set; }
        public string RoutePath { get; set; }
        public string Method { get; set; }
        public string Tag { get; set; }
    }
}
