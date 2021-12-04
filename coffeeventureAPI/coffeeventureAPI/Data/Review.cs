using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Data
{
    public partial class Review
    {
        public Review()
        {
            //UserOrganization = new HashSet<UserOrganization>();
            ReviewImage = new HashSet<ReviewImage>();
            ReviewLike = new HashSet<ReviewLike>();
        }
        public virtual ICollection<ReviewImage> ReviewImage { get; set; }
        public virtual ICollection<ReviewLike> ReviewLike { get; set; }
        public string Id { get; set; }
        public string ShopId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public int? Status { get; set; }
        public int? Rating { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public virtual User User { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
