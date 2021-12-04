using coffeeventureAPI.Dtos.Review;
using coffeeventureAPI.Dtos.Review;
using coffeeventureAPI.Model;
using coffeeventureAPI.Repository.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewEntity = coffeeventureAPI.Data.Review;
using ImageEntity = coffeeventureAPI.Data.Image;
using ReviewLikeEntity = coffeeventureAPI.Data.ReviewLike;
using Microsoft.AspNetCore.Http;

namespace coffeeventureAPI.Service.Review
{
    public interface IReviewService : IScoped
    {
        Task<ReviewDto> AddReview(ReviewDto Review);

        Task<bool> DeleteReview(string id);
        Task<List<ReviewEntity>> GetAllReviews(ReviewRequestDto request);
        Task<ReviewDto> Merge(ReviewDto dto);
        Task<List<ReviewEntity>> GetAllReviewsById(); 
         Task<IEnumerable<ReviewDto>> Select(ReviewRequestDto request);
        Task<IEnumerable<ReviewDto>> SelectReviewsByShop(ReviewRequestDto request);
        Task<int> Count(ReviewRequestDto request);
        Task<List<ImageEntity>> Upload(IFormFileCollection files, string ReviewId);
        Task<ReviewDto> GetReviewById(string id);
        Task<bool> Like(ReviewDto dto);
        Task<bool> UpdateReview(ReviewEntity Review);
        Task<IEnumerable<ReviewLikeEntity>> SelectUsersById( string id);
    }
}
