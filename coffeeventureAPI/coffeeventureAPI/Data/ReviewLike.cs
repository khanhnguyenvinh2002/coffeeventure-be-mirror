using System;

namespace coffeeventureAPI.Data
{
    public partial class ReviewLike
    {
        public string Id { get; set; }
        public string ReviewId { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Review Review { get; set; }
    }
}
