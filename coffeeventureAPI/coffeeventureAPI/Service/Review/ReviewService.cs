//using coffeeventureAPI.Core.Base;
using coffeeventureAPI.Dtos.Review;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Repository.Review;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageEntity = coffeeventureAPI.Data.Image;
using ReviewEntity = coffeeventureAPI.Data.Review;
using ReviewLikeEntity = coffeeventureAPI.Data.ReviewLike;

namespace coffeeventureAPI.Service.Review
{
    public class ReviewService : IReviewService
    {
        #region Private variables

        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private variables

        public ReviewService(IReviewRepository reviewRepository, IUnitOfWork unitOfWork) : base()
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ReviewDto> AddReview(ReviewDto Review)
        {
            return await _reviewRepository.AddReview(Review);
        }

        public async Task<bool> DeleteReview(string id)
        {
            return await _reviewRepository.DeleteReview(id);
        }

        public async Task<List<ReviewEntity>> GetAllReviews(ReviewRequestDto request)
        {
            return await _reviewRepository.GetAllReviews(request);
        }
        public async Task<List<ReviewEntity>> GetAllReviewsById()
        {
            return await _reviewRepository.GetAllReviewsById();
        }
        public async Task<ReviewDto> Merge(ReviewDto dto)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();

            // Merge Shop
            var dtoResult = await _reviewRepository.Merge(dto);

            // Commit transaction
            transaction.Commit();
            return await _reviewRepository.GetReviewById(dto.Id);
        }
        public async Task<List<ImageEntity>> Upload(IFormFileCollection files, string ReviewId)
        {

            List<ImageEntity> result = new List<ImageEntity>();

            foreach (var file in files)
            {
                ImageEntity fileInfo = await _reviewRepository.Upload(file, ReviewId);
                result.Add(fileInfo);
            }
            return result; 
        }
        //[RedisCache(Duration = 1, Measure = TimeMeasure.Day)]
        public async Task<IEnumerable<ReviewDto>> Select(ReviewRequestDto request)
        {
            return await _reviewRepository.Select(request);
        }
        public async Task<IEnumerable<ReviewDto>> SelectReviewsByShop(ReviewRequestDto request)
        {
            return await _reviewRepository.SelectReviewsByShop(request);
        }
        public async Task<int> Count(ReviewRequestDto request)
        {
            return await _reviewRepository.Count(request);
        }
        public async Task<ReviewDto> GetReviewById(string id)
        {
            return await _reviewRepository.GetReviewById(id);
        }
        public async Task<bool> Like( ReviewDto dto)
        {
            return await _reviewRepository.Like(dto);
        }
        public async Task<bool> UpdateReview(ReviewEntity Review)
        {
            return await _reviewRepository.UpdateReview(Review);
        }
        public async Task<IEnumerable<ReviewLikeEntity>> SelectUsersById( string id)
        {
            return await _reviewRepository.SelectUsersById(id);
        }
    }
}
