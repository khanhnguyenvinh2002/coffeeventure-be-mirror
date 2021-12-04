using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class Role
    {
        public Role()
        {
            UserRole = new HashSet<UserRole>();
            RoleOperation = new HashSet<RoleOperation>();
        }

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public short? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<RoleOperation> RoleOperation { get; set; }
    }
}
