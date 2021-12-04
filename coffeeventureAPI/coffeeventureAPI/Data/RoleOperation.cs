using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class RoleOperation
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string OperationId { get; set; }
        public virtual Role Role { get; set; }
        public virtual Operation Operation { get; set; }
    }
}
