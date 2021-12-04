using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Service.Blob;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using ImageEntity = coffeeventureAPI.Data.Image;

namespace coffeeventureAPI.Repository.Image
{
    public class ImageRepository : BaseRepository, IImageRepository
    {
        private string _rootPath = string.Empty;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private IBlobService _blobService;

        #region Constructor

        public ImageRepository(IUnitOfWork unitOfWork, IConfiguration configuration, IBlobService blobService)
        {
            _blobService = blobService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _rootPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.ToString() + _configuration["UploadLocation:MainPath"];
        }

        #endregion Constructor

        public async Task<ImageEntity> Upload(IFormFile file)
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

                _unitOfWork.Insert(entity);

                // Check exist folder
                //if (!Directory.Exists(pathFolder))
                //{
                //    Directory.CreateDirectory(pathFolder);
                //}

                // Get file name
                //// Copy file to disk
                //using var stream = new FileStream(Path.Combine(_rootPath, id.ToString() + extension), FileMode.Create);
                //file.CopyTo(stream);
            }

            return await Task.FromResult(entity);
        }
    }
}

