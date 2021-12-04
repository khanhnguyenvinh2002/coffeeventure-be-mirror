using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class UserOrganization
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OrgId { get; set; }

        public virtual User User { get; set; }
    }
}
