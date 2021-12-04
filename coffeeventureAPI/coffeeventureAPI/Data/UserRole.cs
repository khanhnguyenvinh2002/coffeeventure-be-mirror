using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class UserRole
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
