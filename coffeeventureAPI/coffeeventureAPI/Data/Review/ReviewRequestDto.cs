using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coffeeventureAPI.Dtos.Base;

namespace coffeeventureAPI.Dtos.Review
{
     public class ReviewRequestDto : BaseRequestDto
    {
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
    }
}
