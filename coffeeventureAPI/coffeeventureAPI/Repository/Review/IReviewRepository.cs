using coffeeventureAPI.Dtos.Review;
using coffeeventureAPI.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewEntity = coffeeventureAPI.Data.Review;
using ImageEntity = coffeeventureAPI.Data.Image;

using ReviewLikeEntity = coffeeventureAPI.Data.ReviewLike;
namespace coffeeventureAPI.Repository.Review
{
    public interface IReviewRepository : IScoped
    {
        Task<List<ReviewEntity>> GetAllReviews(ReviewRequestDto request);
        Task<List<ReviewEntity>> GetAllReviewsById();
        Task<ReviewDto> Merge(ReviewDto model);
        Task<ImageEntity> Upload(IFormFile file, string shopId); 
         Task<IEnumerable<ReviewDto>> Select(ReviewRequestDto request);
        Task<IEnumerable<ReviewDto>> SelectReviewsByShop(ReviewRequestDto request);
        Task<int> Count(ReviewRequestDto request);
        Task<ReviewDto> GetReviewById(string id);
        Task<ReviewDto> AddReview(ReviewDto Review);
        Task<bool> Like(ReviewDto dto);
        Task<bool> UpdateReview(ReviewEntity Review);
        Task<bool> DeleteReview(string id);
        Task<IEnumerable<ReviewLikeEntity>> SelectUsersById(string id);
    }
}
