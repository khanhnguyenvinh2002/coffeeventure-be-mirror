

using coffeeventureAPI.Dtos.Base;
using ActionEntity = coffeeventureAPI.Data.Action;
namespace coffeeventureAPI.Data
{
    public class ActionRequestDto : BaseRequestDto
    {
        public string Id { get; set; }
        public string InOutType { get; set; }
        public string DataType { get; set; }
        public string Name { get; set; }
        public string ExcludeOperationId { get; set; }
        public string IncludeOperationId { get; set; }
        public string RoutePath { get; set; }
        public string Method { get; set; }
        public string Tag { get; set; }
    }
}
