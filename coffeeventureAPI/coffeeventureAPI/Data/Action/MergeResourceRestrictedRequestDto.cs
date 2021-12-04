using System;
using System.Collections.Generic;
using System.Text;
using ResourceRestrictedEntity = coffeeventureAPI.Data.ResourceRestricted;

namespace coffeeventureAPI.Data
{
    public class MergeResourceRestrictedRequestDto
    {
        public string RoleId { get; set; }
        public string OperationId { get; set; }
        public IEnumerable<ResourceRestrictedEntity> ResourceRestricted { get; set; }
    }
}
