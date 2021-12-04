using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class Operation
    {
        public Operation()
        {
            InverseParentMenuNavigation = new HashSet<Operation>();
            RoleOperation = new HashSet<RoleOperation>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public string Link { get; set; }
        public bool Type { get; set; }
        public string ParentMenu { get; set; }
        public int MenuOrder { get; set; }
        public string MenuIcon { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual Operation ParentMenuNavigation { get; set; }
        public virtual ICollection<Operation> InverseParentMenuNavigation { get; set; }
        public virtual ICollection<RoleOperation> RoleOperation { get; set; }
    }
}
