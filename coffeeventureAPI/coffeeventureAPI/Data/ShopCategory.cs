using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Data
{
   public partial class ShopCategory
    {
        public string Id { get; set; }
        public string ShopId { get; set; }
        public string CategoryId { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual Category Category { get; set; }
    }
}
