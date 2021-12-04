
using coffeeventureAPI.Core.Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewEntity = coffeeventureAPI.Data.Review;

namespace coffeeventureAPI.Dtos.Review
{
    public class ReviewDto : BaseModel
    {
        public ReviewDto()
        {

        }
        public ReviewDto(ReviewEntity entity) : base(entity)
        {
        }
        public string Id { get; set; }
        public string ShopName { get; set; }
        public string ShopId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public IEnumerable<string> LikedUsers { get; set; }
        public int Likes { get; set; }
        public int? Status { get; set; }
        public int? Rating { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string AvatarPath { get; set; }
        public IEnumerable<string> ImageDirectories { get; set; }
    }
}
