
using coffeeventureAPI.Dtos.Base;

namespace coffeeventureAPI.Data
{
    public class OperationRequestDto : BaseRequestDto
    {
        public string Id { get; set; }
        public string[] ExcludeIds { get; set; }
        public string Name { get; set; }
        public string ParentMenu { get; set; }
        public bool? Type { get; set; }
        public string Method { get; set; }
    }
}
