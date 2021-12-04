using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class ResourceRestricted
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string OperationId { get; set; }
        public string ResourceId { get; set; }
        public byte? Type { get; set; }
    }
}
