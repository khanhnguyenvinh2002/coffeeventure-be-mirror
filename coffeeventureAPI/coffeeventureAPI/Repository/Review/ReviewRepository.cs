using coffeeventureAPI.Dtos.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ReviewEntity = coffeeventureAPI.Data.Review;
using ReviewLikeEntity = coffeeventureAPI.Data.ReviewLike;
using ImageEntity = coffeeventureAPI.Data.Image;
using UserEntity = coffeeventureAPI.Data.User;
using ShopEntity = coffeeventureAPI.Data.Shop;
using ReviewImageEntity = coffeeventureAPI.Data.ReviewImage;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Core.Repository;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using coffeeventureAPI.Service.Blob;

namespace coffeeventureAPI.Repository.Review
{
    public class ReviewRepository : BaseRepository, IReviewRepository
    {
        private IBlobService _blobService;

        private string _rootPath = string.Empty;
        private IConfiguration Configuration;
        private string connectionString;
        private IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewRepository> _logger;
        public ReviewRepository(IConfiguration configuration, ILogger<ReviewRepository> logger, IUnitOfWork unitOfWork, IBlobService blobService)
        {
            _blobService = blobService;
            Configuration = configuration;
            connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            _logger = logger;
            _unitOfWork = unitOfWork;
            _rootPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.ToString() + Configuration["UploadLocation:MainPath"];
        }
        public async Task<ReviewDto> AddReview(ReviewDto Review)
        {
            Review.Id = Guid.NewGuid().ToString("N");
            var query = _unitOfWork.Select<ReviewEntity>();
            _unitOfWork.Merge<ReviewEntity, ReviewDto>(Review);
            return await Task.FromResult(Review);
        }

        public async Task<IEnumerable<ReviewLikeEntity>> SelectUsersById(string id)
        {
            var likedUsers = _unitOfWork.Select<ReviewLikeEntity>().Where(x => x.ReviewId == id);
            return await Task.FromResult(likedUsers.ToList());
        }
        public async Task<bool> DeleteReview(string id)
        {
            var query = _unitOfWork.Select<ReviewEntity>().Where(x => x.Id == id).FirstOrDefault();
            var query3 = _unitOfWork.Select<ReviewImageEntity>().Where(x => x.ReviewId == id).AsNoTracking();
            var query4 = _unitOfWork.Select<ReviewLikeEntity>().Where(x => x.ReviewId == id).AsNoTracking();

            _unitOfWork.BulkDelete(query4);
            _unitOfWork.BulkDelete(query3);
            _unitOfWork.Delete(query);
            return await Task.FromResult(true);
        }
        
        public async Task<IEnumerable<ReviewDto>> SelectReviewsByShop(ReviewRequestDto request)
        {
            List<string> temp = new List<string>();
            IQueryable<ReviewEntity> query = _unitOfWork.Select<ReviewEntity>().AsNoTracking().Where(x => x.ShopId == request.ShopId).Where(x=>x.Status==1);
            var jimage = _unitOfWork.Select<ReviewImageEntity>().AsNoTracking();
            var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            query = Filter(query, request).OrderByDescending(x => x.CreatedAt);
            query = query.Paging(request);
            var ans = query.Select(x => new ReviewDto(x)).ToList();
            foreach (var i in ans)
            {
                var imageIds = jimage.Where(x => x.ReviewId == i.Id).Select(x => x.ImageId).ToList();
                foreach (var imageId in imageIds)
                {
                    if (!string.IsNullOrEmpty(imageId))
                        temp.Add(images.Where(x => x.Id == imageId).Select(x => x.Path).FirstOrDefault());
                }
                i.ImageDirectories = temp;
            }
            return await Task.FromResult(ans.OrderByDescending(x => x.CreatedAt));
        }
        public async Task<IEnumerable<ReviewDto>> Select(ReviewRequestDto request)
        {
            List<string> temp = new List<string>();
            IQueryable<ReviewEntity> query = _unitOfWork.Select<ReviewEntity>().AsNoTracking().Where(x => x.CreatedBy == _unitOfWork.GetCurrentUserId());
            var jimage = _unitOfWork.Select<ReviewImageEntity>().AsNoTracking();
            var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            query = Filter(query, request).OrderByDescending(x => x.CreatedAt);
            query = query.Paging(request);
            var ans = query.Select(x => new ReviewDto(x)).ToList();
            foreach (var i in ans)
            {
                var imageIds = jimage.Where(x => x.ReviewId == i.Id).Select(x => x.ImageId).ToList();
                foreach (var imageId in imageIds)
                {
                    if (!string.IsNullOrEmpty(imageId))
                        temp.Add(images.Where(x => x.Id == imageId).Select(x => x.Path).FirstOrDefault());
                }
                i.ImageDirectories = temp;
            }
            return await Task.FromResult(ans.OrderByDescending(x => x.CreatedAt));
        }
        public async Task<int> Count(ReviewRequestDto request)
        {
            IQueryable<ReviewEntity> query = _unitOfWork.Select<ReviewEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }
        public async Task<ReviewDto> Merge(ReviewDto model)
        {
            var result = _unitOfWork.Merge<ReviewEntity, ReviewDto>(model);
            return await Task.FromResult(result);
        }
        public async Task<List<ReviewEntity>> GetAllReviews(ReviewRequestDto request)
        {
            var query = _unitOfWork.Select<ReviewEntity>().AsNoTracking().Where(x => x.Status == 1).OrderByDescending(x => x.CreatedAt);
            var ans = Filter(query, request);
            if (!string.IsNullOrEmpty(request.CreatedBy))
            {
                ans = ans.Where(x => x.CreatedBy == request.CreatedBy);
            }
            var Reviews = ans.Paging(request).ToList();
            return await Task.FromResult(Reviews);
        }
        public async Task<List<ReviewEntity>> GetAllReviewsById()
        {
            List<ReviewEntity> Reviews = _unitOfWork.Select<ReviewEntity>().Where(x => x.CreatedBy == _unitOfWork.GetCurrentUserId()).ToList();
            return await Task.FromResult(Reviews);
        }
        public async Task<ImageEntity> Upload(IFormFile file, string ReviewId)
        {

            var entity = new ImageEntity();
            if (file.Length > 0)
            {
                var id = Guid.NewGuid().ToString("N");
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var result = await _blobService.UploadFileBlobAsync("coffeeventurecontainer", file.OpenReadStream(), file.ContentType, fileName);
                entity.Id = id;
                entity.Name = fileName;
                entity.Path = result.AbsoluteUri;
                entity.Size = (int?)file.Length;
                var fileImage = new ReviewImageEntity() { Id = Guid.NewGuid().ToString("N"), ImageId = entity.Id, ReviewId = ReviewId };
                _unitOfWork.Insert(entity);
                _unitOfWork.Insert(fileImage);
            }

            return await Task.FromResult(entity);
        }
        //close con automatically using "using" block
        //using (SqlConnection con = new SqlConnection(connectionString))
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand("[dbo].[spSelectReview]", con);
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //        ReviewEntity Review = new ReviewEntity();
        //        // rdr["id] serves as index
        //        Review.Status = Convert.ToInt32(rdr["Status"]);
        //        Review.Id = rdr["Id"].ToString();
        //        Review.Content = rdr["Content"].ToString();
        //        Reviews.Add(Review);
        //        }
        //        rdr.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error at GetAllCustomers() : (");
        //        Reviews = null;

        //    }
        //}
        //return await Task.FromResult(Reviews);

        public async Task<ReviewDto> GetReviewById(string id)
        {

            var ans = _unitOfWork.Select<ReviewEntity>().Where(x => x.Id == id).Select(x => new ReviewDto(x)).FirstOrDefault();
            List<string> temp = new List<string>();
            var shop = _unitOfWork.Select<ShopEntity>().Where(x => x.Id == ans.ShopId).Select(x=> x.Name).FirstOrDefault();
            var jimage = _unitOfWork.Select<ReviewImageEntity>().AsNoTracking();
            var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            var likedUsers = _unitOfWork.Select<ReviewLikeEntity>().Where(x=> x.ReviewId == id).Select(x=> x.CreatedBy);
            var likes = likedUsers.Count();
            var user = _unitOfWork.Select<UserEntity>().Where(x => x.Id == ans.CreatedBy).FirstOrDefault();
            ans.Likes = likes;
            ans.LikedUsers = likedUsers.ToList();
            ans.ShopName = shop;
            var imageIds = jimage.Where(x => x.ReviewId == ans.Id).Select(x => x.ImageId).ToList();
            foreach (var imageId in imageIds)
            {
                if (!string.IsNullOrEmpty(imageId))
                    temp.Add(images.Where(x => x.Id == imageId).Select(x => x.Path).FirstOrDefault());
            }
            ans.ImageDirectories = temp;
            ans.UserName = user.UserName;
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                ans.AvatarPath = user.Avatar;
            }
            return await Task.FromResult(ans);
        }
        //ReviewEntity Review = new ReviewEntity();
        //    //close con automatically using "using" block
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("[dbo].[spSelectReviewById]", con);
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            con.Open();
        //            cmd.Parameters.AddWithValue("@Id", id);
        //            SqlDataReader rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //            // rdr["id] serves as index
        //            Review.Id = id;
        //            Review.Content = rdr["Content"].ToString();
        //            Review.Status = Convert.ToInt32(rdr["Status"]);
        //        }
        //            rdr.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.LogError(e, "Error at GetCustomerById() : (");
        //        Review = null;

        //        }
        //    }
        //    return await Task.FromResult(Review);

        public async Task<bool> Like(ReviewDto dto)
        {
            var query = _unitOfWork.Select<ReviewEntity>().AsNoTracking().Where(x => x.Id == dto.Id).FirstOrDefault();
            var user = _unitOfWork.Select<ReviewLikeEntity>().Where(x => x.ReviewId == dto.Id).Where(x => x.CreatedBy == _unitOfWork.GetCurrentUserId()).FirstOrDefault();
            if(user == null)
            {
                var like = new ReviewLikeEntity() { Id = Guid.NewGuid().ToString("N"), ReviewId = dto.Id, Status = dto.Status };
                _unitOfWork.Insert(like);

            }
            else
            {
                _unitOfWork.Delete(user);
            }
            return await Task.FromResult(true);
        }
        public async Task<bool> UpdateReview(ReviewEntity Review)
        {
            _unitOfWork.Update(Review);

            return await Task.FromResult(true);
            // throw new NotImplementedException();
            //using (SqlConnection con = new SqlConnection(connectionString))
            //    {
            //        try
            //        {
            //            SqlCommand cmd = new SqlCommand("[dbo].[spUpdateCustomer]", con);
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            con.Open();
            //            cmd.Parameters.AddWithValue("@Id", Review.Id);
            //            cmd.Parameters.AddWithValue("@Content", Review.Content);
            //            cmd.Parameters.AddWithValue("@Status", Review.Status);
            //        //cmd.Parameters.AddWithValue("@Created", Review.Created.ToString("yyyy-MM-dd HH:mm:ss"));
            //        cmd.ExecuteNonQuery();
            //        }
            //        catch (Exception e)
            //        {
            //            _logger.LogError(e, "Error at UpdateCustomer() : (");
            //            Review = null;

            //        }
            //    }
            //    return await Task.FromResult(true);
        }
        private IQueryable<ReviewEntity> Filter(IQueryable<ReviewEntity> models, ReviewRequestDto searchEntity)
        {
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.Id == searchEntity.Id);
            }

            if (!string.IsNullOrEmpty(searchEntity.CreatedBy))
            {
                models = models.Where(x => x.CreatedBy == searchEntity.CreatedBy);
            }

            if (!string.IsNullOrEmpty(searchEntity.ShopId))
            {
                models = models.Where(x => x.ShopId == searchEntity.ShopId);
            }

            if (!string.IsNullOrEmpty(searchEntity.Content))
            {
                models = models.Where(x => x.Content == searchEntity.Content);
            }
            if (searchEntity.Status != null)
            {
                models = models.Where(x => x.Status == searchEntity.Status);
            }
            if (searchEntity.Rating != null)
            {
                models = models.Where(x => x.Rating == searchEntity.Rating);
            }

            return models;
        }

    }

}
