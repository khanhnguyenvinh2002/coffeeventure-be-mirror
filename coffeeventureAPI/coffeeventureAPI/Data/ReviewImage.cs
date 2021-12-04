using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace coffeeventureAPI.Data
{
    public partial class ReviewImage
    {
        public string Id { get; set; }
        public string ReviewId { get; set; }
        //[ForeignKey("ReviewImage")]
        public string ImageId { get; set; }

        public virtual Review Review { get; set; }
        public virtual Image Image { get; set; }
    }
}
