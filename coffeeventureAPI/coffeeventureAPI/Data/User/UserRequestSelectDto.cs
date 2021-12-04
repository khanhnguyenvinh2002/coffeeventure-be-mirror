
using coffeeventureAPI.Dtos.Base;
using System.Collections.Generic;

namespace coffeeventureAPI.Data.UserDto
{
    public class UserRequestSelectDto : BaseRequestDto
    {
        public IEnumerable<string> Ids { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        public string[] ExcludeIds { get; set; }
        public string Email { get; set; }
        public string OrgId { get; set; }
        public string FullName { get; set; }
        public string GeneralFilter { get; set; }
        public string RoleId { get; set; }
    }
}
