using coffeeventureAPI.Dtos.Journal;
using coffeeventureAPI.Model;
using coffeeventureAPI.Model.unitsOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using JournalLikeEntity = coffeeventureAPI.Data.JournalLike;
using JournalEntity = coffeeventureAPI.Data.Journal;
using JournalShopEntity = coffeeventureAPI.Data.JournalShop;
using ImageEntity = coffeeventureAPI.Data.Image;
using UserEntity = coffeeventureAPI.Data.User;
using JournalImageEntity = coffeeventureAPI.Data.JournalImage;
using System.Data.Entity;
using coffeeventureAPI.Core.Repository;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Reflection;
using coffeeventureAPI.Service.Blob;

namespace coffeeventureAPI.Repository.Journal
{
    public class JournalRepository : BaseRepository, IJournalRepository
    {
            private string _rootPath = string.Empty;
            private IConfiguration Configuration;
        private IBlobService _blobService;
            private string connectionString;
            private IUnitOfWork _unitOfWork;
            private readonly ILogger<JournalRepository> _logger;
            public JournalRepository(IConfiguration configuration, ILogger<JournalRepository> logger, IUnitOfWork unitOfWork, IBlobService blobService)
            {
            _blobService = blobService;
                Configuration = configuration;
                connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                _logger = logger;
                _unitOfWork = unitOfWork;
            _rootPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.ToString() + Configuration["UploadLocation:MainPath"];
        }
        //public async Task<JournalDto> AddJournal(JournalDto journal)
        //{
        //    journal.Id = Guid.NewGuid().ToString("N");
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("[dbo].[spInsertIntoJournal]", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            con.Open();
        //            cmd.Parameters.AddWithValue("@Id", journal.Id);
        //            cmd.Parameters.AddWithValue("@Content", journal.Content);
        //            cmd.Parameters.AddWithValue("@Status", journal.Status);
        //            cmd.Parameters.AddWithValue("@CreatedBy", _unitOfWork.GetCurrentUserId());
        //            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        //            cmd.Parameters.AddWithValue("@Feeling", "journal.Feeling");
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.LogError(e, "Error at AddCustomer() : (");
        //            journal = null;

        //        }
        //    }

        //    return await Task.FromResult(journal);
        //}
        //}

        public async Task<JournalDto> AddJournal(JournalDto journal)
        {
            journal.Id = Guid.NewGuid().ToString("N");
            var query = _unitOfWork.Select<JournalEntity>();
            _unitOfWork.Merge<JournalEntity, JournalDto>(journal);
            return await Task.FromResult(journal);
        }

        public async Task<IEnumerable<JournalLikeEntity>> SelectUsersById(string id)
        {
            var likedUsers = _unitOfWork.Select<JournalLikeEntity>().Where(x=> x.JournalId == id);
            return await Task.FromResult(likedUsers.ToList());
        }
        public async Task<bool> DeleteJournal(string id)
        {
            var query = _unitOfWork.Select<JournalEntity>().Where(x => x.Id == id).FirstOrDefault();
            var query2 = _unitOfWork.Select<JournalShopEntity>().Where(x => x.JournalId == id).AsNoTracking();
            var query3 = _unitOfWork.Select<JournalImageEntity>().Where(x => x.JournalId == id).AsNoTracking();
            var query4 = _unitOfWork.Select<JournalLikeEntity>().Where(x => x.JournalId == id).AsNoTracking();
            _unitOfWork.BulkDelete(query2);
            _unitOfWork.BulkDelete(query3);
            _unitOfWork.BulkDelete(query4);
            _unitOfWork.Delete(query);
            return await Task.FromResult(true);
            }
        public async Task<IEnumerable<JournalDto>> Select(JournalRequestDto request)
        {
            List<string> temp = new List<string>();
            IQueryable<JournalEntity> query = _unitOfWork.Select<JournalEntity>().AsNoTracking().Where(x => x.CreatedBy == _unitOfWork.GetCurrentUserId());
            var jimage = _unitOfWork.Select<JournalImageEntity>().AsNoTracking();
            var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            query = Filter(query, request).OrderByDescending(x=> x.CreatedAt);
            query = query.Paging(request);
            var ans = query.Select(x => new JournalDto(x)).ToList();
            foreach (var i in ans)
            {
                var imageIds = jimage.Where(x => x.JournalId == i.Id).Select(x => x.ImageId).ToList();
                foreach (var imageId in imageIds)
                {
                    if (!string.IsNullOrEmpty(imageId))
                        temp.Add(images.Where(x => x.Id == imageId).Select(x => x.Path).FirstOrDefault());
                }
                i.ImageDirectories = temp;
            }
            return await Task.FromResult(ans.OrderByDescending(x=>x.CreatedAt));
        }
        public async Task<int> Count(JournalRequestDto request)
        {
            IQueryable<JournalEntity> query = _unitOfWork.Select<JournalEntity>().AsNoTracking();
            query = Filter(query, request);
            return await query.CountAsync();
        }
        public async Task<JournalDto> Merge(JournalDto model)
        {
            var result = _unitOfWork.Merge<JournalEntity, JournalDto>(model);
            return await Task.FromResult(result);
        }
        public async Task<List<JournalDto>> GetAllJournals(JournalRequestDto request)
        {
            var query = _unitOfWork.Select<JournalEntity>().AsNoTracking().Where(x=> x.Status == 1).OrderByDescending(x => x.CreatedAt);
            var ans = Filter(query, request).Select(x => new JournalDto(x));
            var journals = ans.Paging(request).ToList();
            return await Task.FromResult(journals);
        }

        public async Task<bool> Like(JournalDto dto)
        {
           var query = _unitOfWork.Select<JournalEntity>().AsNoTracking().Where(x=> x.Id == dto.Id).FirstOrDefault();

            var user = _unitOfWork.Select<JournalLikeEntity>().Where(x => x.JournalId == dto.Id).Where(x => x.CreatedBy == _unitOfWork.GetCurrentUserId()).FirstOrDefault();
            if (user == null)
            {
                var like = new JournalLikeEntity() { Id = Guid.NewGuid().ToString("N"), JournalId = dto.Id, Status = dto.Status };
                _unitOfWork.Insert(like);
            }

            else
            {
                _unitOfWork.Delete(user);
            }
            return await Task.FromResult(true);
        }
        public async Task<List<JournalDto>> GetAllJournalsById()
        {
            var journals = _unitOfWork.Select<JournalEntity>().Where(x=> x.CreatedBy == _unitOfWork.GetCurrentUserId()).Select(x => new JournalDto(x)).ToList();
            return await Task.FromResult(journals);
        }
        //public async Task<ImageEntity> Upload(IFormFile file, string journalId)
        //{
        //    var entity = new ImageEntity();
        //    //_unitOfWork.BulkDelete(_unitOfWork.Select<JournalImageEntity>().Where(x => x.JournalId == journalId));
        //    // Copy file to file server and rename with date time information
        //    if (file.Length > 0)
        //    {
        //        var id = Guid.NewGuid().ToString("N");
        //        var folderName = Path.Combine("Resources", "Images");
        //        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //        var fullPath = Path.Combine(pathToSave, fileName);
        //        var dbPath = Path.Combine(folderName, fileName);
        //        entity.Id = id;
        //        entity.Name = fileName;
        //        entity.Path = "https://coffeeventure.azurewebsites.net/" + dbPath;
        //        entity.Size = (int?)file.Length;
        //        var fileImage = new JournalImageEntity() { Id = Guid.NewGuid().ToString("N"), ImageId = entity.Id, JournalId = journalId };
        //        _unitOfWork.Insert(entity);
        //        _unitOfWork.Insert(fileImage);
        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //        }
        //    }

        //    return await Task.FromResult(entity);
        //}
        public async Task<ImageEntity> Upload(IFormFile file, string journalId)
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
                var fileImage = new JournalImageEntity() { Id = Guid.NewGuid().ToString("N"), ImageId = entity.Id, JournalId = journalId };
                _unitOfWork.Insert(entity);
                _unitOfWork.Insert(fileImage);
            }

            return await Task.FromResult(entity);
        }


        //  public async Task<ImageEntity> Upload(IFormFile file, string journalId)
        //{
        //    var entity = new ImageEntity();

        //    // Copy file to file server and rename with date time information
        //    if (file.Length > 0)
        //    {
        //        var id = Guid.NewGuid().ToString("N");

        //        // Get file name
        //        //    var folderName = Path.Combine("Resources", "Images");
        //        //    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        //    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //        //    var fullPath = Path.Combine(pathToSave, fileName);
        //        //    var dbPath = Path.Combine(folderName, fileName);
        //        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

        //        var fullPath = Path.Combine("files",fileName);

        //         var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), fullPath);
        //        // Insert file infomation to database
        //        entity.Id = id;
        //        entity.Name = fileName;
        //        entity.Path = "https://coffeeventure.azurewebsites.net/" +fullPath;

        //        entity.Size = (int?)file.Length;
        //        var fileImage = new JournalImageEntity() { Id = Guid.NewGuid().ToString("N"), ImageId = entity.Id, JournalId = journalId };
        //        _unitOfWork.Insert(entity);
        //        _unitOfWork.Insert(fileImage);

        //        // Check exist folder
        //        if (!Directory.Exists(fullPath))
        //        {
        //            Directory.CreateDirectory(fullPath);
        //        }

        //        // Copy file to disk
        //        using (var stream = new FileStream(Path.Combine(_rootPath, fileName), FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //        }
        //    }
        //    //var id = Guid.NewGuid().ToString("N");
        //    //    var folderName = Path.Combine("Resources", "Images");
        //    //    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //    //    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //    //    var fullPath = Path.Combine(pathToSave, fileName);
        //    //    var dbPath = Path.Combine(folderName, fileName);
        //    //    entity.Id = id;
        //    //    entity.Name = fileName;
        //    //    entity.Path = "https://coffeeventure.azurewebsites.net/" + dbPath;
        //    //    entity.Size = (int?)file.Length;
        //    //    var fileImage = new JournalImageEntity() { Id = Guid.NewGuid().ToString("N"), ImageId = entity.Id, JournalId = journalId };
        //    //    _unitOfWork.Insert(entity);
        //    //    _unitOfWork.Insert(fileImage);
        //    //    using (var stream = new FileStream(fullPath, FileMode.Create))
        //    //    {
        //    //        file.CopyTo(stream);
        //    //    }


        //    return await Task.FromResult(entity);
        //}
        //public async Task<ImageEntity> Upload(IFormFile file, string journalId)
        //{
        //    var entity = new ImageEntity();
        //    _unitOfWork.BulkDelete(_unitOfWork.Select<JournalImageEntity>().Where(x => x.JournalId == journalId));
        //    // Copy file to file server and rename with date time information
        //    if (file.Length > 0)
        //    {
        //        var id = Guid.NewGuid().ToString("N");

        //        // Get file name
        //        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //        string extension = Path.GetExtension(fileName);

        //        // Insert file infomation to database
        //        entity.Id = id;
        //        entity.Name = fileName;
        //        entity.Path = Path.Combine(_rootPath, id.ToString() + extension);
        //        entity.Size = (int?)file.Length;
        //        var fileImage = new JournalImageEntity() { Id = Guid.NewGuid().ToString("N"), ImageId = entity.Id, JournalId = journalId };
        //        _unitOfWork.Insert(entity);
        //        _unitOfWork.Insert(fileImage);

        //        // Check exist folder
        //        //if (!Directory.Exists(pathFolder))
        //        //{
        //        //    Directory.CreateDirectory(pathFolder);
        //        //}

        //        // Copy file to disk
        //        using var stream = new FileStream(Path.Combine(_rootPath, id.ToString() + extension), FileMode.Create);
        //        file.CopyTo(stream);
        //    }

        //    return await Task.FromResult(entity);
        //}
        //close con automatically using "using" block
        //using (SqlConnection con = new SqlConnection(connectionString))
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand("[dbo].[spSelectJournal]", con);
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //        JournalEntity journal = new JournalEntity();
        //        // rdr["id] serves as index
        //        journal.Status = Convert.ToInt32(rdr["Status"]);
        //        journal.Id = rdr["Id"].ToString();
        //        journal.Content = rdr["Content"].ToString();
        //        journals.Add(journal);
        //        }
        //        rdr.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error at GetAllCustomers() : (");
        //        journals = null;

        //    }
        //}
        //return await Task.FromResult(journals);

        public async Task<JournalDto> GetJournalById(string id)
    {
            var ans = _unitOfWork.Select<JournalEntity>().Where(x => x.Id == id).Select(x => new JournalDto(x)).FirstOrDefault();
            List<string> temp = new List<string>();
            var jimage = _unitOfWork.Select<JournalImageEntity>().AsNoTracking();
            var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            var likedUsers = _unitOfWork.Select<JournalLikeEntity>().Where(x=> x.JournalId == id).Select(x => x.CreatedBy);
            var likes = likedUsers.Count();
            var user = _unitOfWork.Select<UserEntity>().Where(x => x.Id == ans.CreatedBy).FirstOrDefault();
            ans.Likes = likes;
            ans.LikedUsers = likedUsers.ToList();
            var imageIds = jimage.Where(x => x.JournalId == ans.Id).Select(x => x.ImageId).ToList();
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
            //var ans = _unitOfWork.Select<JournalEntity>().Where(x => x.Id == id).Select(x=> new JournalDto(x)).FirstOrDefault();
            //List<byte[]> temp = new List<byte[]>();
            //var jimage = _unitOfWork.Select<JournalImageEntity>().AsNoTracking();
            //var images = _unitOfWork.Select<ImageEntity>().AsNoTracking();
            //var user = _unitOfWork.Select<UserEntity>().Where(x => x.Id == ans.CreatedBy).FirstOrDefault();

            //    var imageIds = jimage.Where(x => x.JournalId == ans.Id).Select(x => x.ImageId).ToList();
            //    foreach (var imageId in imageIds)
            //    {
            //        if (!string.IsNullOrEmpty(imageId))
            //            temp.Add(File.ReadAllBytes(images.Where(x => x.Id == imageId).Select(x => x.Path).FirstOrDefault()));
            //    }
            //    ans.ImagePaths = temp;
            //ans.UserName = user.UserName;
            //if (!string.IsNullOrEmpty(user.Avatar))
            //{
            //    ans.AvatarPath = File.ReadAllBytes(user.Avatar);
            //}
            //return await Task.FromResult(ans);
        }
        //JournalEntity journal = new JournalEntity();
        //    //close con automatically using "using" block
        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("[dbo].[spSelectJournalById]", con);
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            con.Open();
        //            cmd.Parameters.AddWithValue("@Id", id);
        //            SqlDataReader rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //            // rdr["id] serves as index
        //            journal.Id = id;
        //            journal.Content = rdr["Content"].ToString();
        //            journal.Status = Convert.ToInt32(rdr["Status"]);
        //        }
        //            rdr.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.LogError(e, "Error at GetCustomerById() : (");
        //        journal = null;

        //        }
        //    }
        //    return await Task.FromResult(journal);
    

            public async Task<bool> UpdateJournal(JournalEntity journal)
        {
            _unitOfWork.Update(journal);

            return await Task.FromResult(true);
            // throw new NotImplementedException();
            //using (SqlConnection con = new SqlConnection(connectionString))
            //    {
            //        try
            //        {
            //            SqlCommand cmd = new SqlCommand("[dbo].[spUpdateCustomer]", con);
            //            cmd.CommandType = CommandType.StoredProcedure;
            //            con.Open();
            //            cmd.Parameters.AddWithValue("@Id", journal.Id);
            //            cmd.Parameters.AddWithValue("@Content", journal.Content);
            //            cmd.Parameters.AddWithValue("@Status", journal.Status);
            //        //cmd.Parameters.AddWithValue("@Created", journal.Created.ToString("yyyy-MM-dd HH:mm:ss"));
            //        cmd.ExecuteNonQuery();
            //        }
            //        catch (Exception e)
            //        {
            //            _logger.LogError(e, "Error at UpdateCustomer() : (");
            //            journal = null;

            //        }
            //    }
            //    return await Task.FromResult(true);
            }
        private IQueryable<JournalEntity> Filter(IQueryable<JournalEntity> models, JournalRequestDto searchEntity)
        {
            if (!string.IsNullOrEmpty(searchEntity.Id))
            {
                models = models.Where(x => x.Id == searchEntity.Id);
            }

            if (!string.IsNullOrEmpty(searchEntity.Content))
            {
                models = models.Where(x => x.Content == searchEntity.Content);
            }
            if (searchEntity.Status!=null)
            {
                models = models.Where(x => x.Status == searchEntity.Status);
            }
            if (!string.IsNullOrEmpty(searchEntity.Feeling))
            {
                models = models.Where(x => x.Feeling == searchEntity.Feeling);
            }
            if (!string.IsNullOrEmpty(searchEntity.CreatedBy))
            {
                models = models.Where(x => x.CreatedBy == searchEntity.CreatedBy);
            }

            return models;
        }

    }
    }

