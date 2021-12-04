
using System.Collections.Generic;
using RoleEntity = coffeeventureAPI.Data.Role;
using System.Linq;
using coffeeventureAPI.Core.Base.Model;

namespace coffeeventureAPI.Data
{
    public class RoleDto : BaseModel
    {
        public RoleDto()
        {
        }

        public RoleDto(RoleEntity entity) : base(entity)
        {
        }

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public short? IsActive { get; set; }

        public IEnumerable<UserDtox> User { get; set; }
    }

    public class UserDtox
    {
        public UserDtox()
        {
        }

        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
