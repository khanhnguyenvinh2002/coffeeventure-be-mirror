using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Data
{
    public partial class Category { 
    public Category()
    {
        //UserOrganization = new HashSet<UserOrganization>();
        ShopCategory = new HashSet<ShopCategory>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        //public virtual ICollection<UserOrganization> UserOrganization { get; set; }
        public virtual ICollection<ShopCategory> ShopCategory { get; set; }
}
}
