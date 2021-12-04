using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace coffeeventureAPI.Data
{
    public partial class ShopImage
    {
        public string Id { get; set; }
        public string ShopId { get; set; }
        //[ForeignKey("ShopImages")]
        public string ImageId { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual Image Image { get; set; }
    }
}
