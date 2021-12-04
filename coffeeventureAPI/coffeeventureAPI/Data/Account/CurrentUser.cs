
using coffeeventureAPI.Core.Base.Model;

namespace coffeeventureAPI.Data
{
    public class CurrentUser : BaseModel
    {
        public CurrentUser() { }
        public CurrentUser(object obj) { }

        public string Id { get; set; }
        public string Account { get; set; }
    }
}
