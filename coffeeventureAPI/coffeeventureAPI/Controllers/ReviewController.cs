using coffeeventureAPI.Dtos.Review;
using coffeeventureAPI.Service.Review;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewEntity = coffeeventureAPI.Data.Review;
using ImageEntity = coffeeventureAPI.Data.Image;
using ReviewLikeEntity = coffeeventureAPI.Data.ReviewLike;
using Microsoft.AspNetCore.Authorization;

namespace coffeeventureAPI.Controllers
{
    [Route("review")]
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService ReviewService)
        {   
            _reviewService = ReviewService;
        }
        [Route("get-all")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<ReviewEntity>> GetAllReviews([FromQuery] ReviewRequestDto request)
        {
            return await _reviewService.GetAllReviews(request);
        }
        [Route("get-all-by-id")]
        [HttpGet]
        [AllowAnonymous]

        public async Task<IEnumerable<ReviewEntity>> GetAllReviewsById()
        {
            return await _reviewService.GetAllReviewsById();
        }

        [HttpGet]
        [AllowAnonymous]
        //[RedisCache(Duration = 1, Measure = TimeMeasure.Day)]
        public async Task<IEnumerable<ReviewDto>> Select([FromQuery] ReviewRequestDto request)
        {
            return await _reviewService.Select(request);
        }

        [Route("select-review-by-shop")]
        [AllowAnonymous]
        [HttpGet]
        //[RedisCache(Duration = 1, Measure = TimeMeasure.Day)]
        public async Task<IEnumerable<ReviewDto>> SelectReviewsByShop([FromQuery] ReviewRequestDto request)
        {
            return await _reviewService.SelectReviewsByShop(request);
        }
            
        [Route("count")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<int> Count([FromQuery] ReviewRequestDto request)
        {
            return await _reviewService.Count(request);
        }
        [Route("{id}")]
        [HttpDelete]
        public async Task<bool> Delete([FromRoute] string id)
        {
            return await _reviewService.DeleteReview(id);
        }

        [HttpPost]
        public Task<ReviewDto> AddReview([FromBody] ReviewDto entity)
        {
            return _reviewService.AddReview(entity);
        }

        [Route("{id}")]
        [HttpGet]
        [AllowAnonymous]
        public Task<ReviewDto> GetReviewById([FromRoute] string id)
        {
            return _reviewService.GetReviewById(id);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<bool> UpdateReview([FromBody] ReviewEntity entity)
        {
            return await _reviewService.UpdateReview(entity);
        }
        [Route("merge")]
        [HttpPost]
        public async Task<ReviewDto> Merge([FromBody] ReviewDto dto)
        {
            return await _reviewService.Merge(dto);
        }
        //public async Task<ReviewDto> Merge()
        //{
        //    var file = Request.Form.Files;
        //    var stream = Request.Headers["body"];
        //    string json = JsonConvert.SerializeObject(stream);
        //    JObject juser = JObject.Parse(stream);
        //    ReviewDto requestDto = new ReviewDto();
        //    if (juser["id"] != null)
        //    {
        //        requestDto.Id = (string)juser["id"];
        //    }
        //    if (juser["content"] != null)
        //    {
        //        requestDto.Content = (string)juser["content"];
        //    }
        //    if (juser["status"] != null)
        //    {
        //        requestDto.Status = (int)juser["status"];
        //    }
        //    if (juser["feeling"] != null)
        //    {
        //        requestDto.Feeling = (string)juser["feeling"];
        //    }

        //    return await _reviewService.Merge(requestDto, file);
        //}
        [Route("like")]
        [HttpPost]
        public async Task<bool> Like([FromBody] ReviewDto dto)
        {
            return await _reviewService.Like(dto);
        }


        [Route("upload-images/{id}")]
        [HttpPost]
        public async Task<List<ImageEntity>> Upload([FromRoute] string id)
        {
            var file = Request.Form.Files;
            return await _reviewService.Upload(file, id);
        }

        [Route("review-like/{id}")]
        [HttpGet]
        [AllowAnonymous]
        //[RedisCache(Duration = 1, Measure = TimeMeasure.Day)]
        public async Task<IEnumerable<ReviewLikeEntity>> SelectUsersById([FromRoute] string id)
        {
            return await _reviewService.SelectUsersById(id);
        }
    }
}
