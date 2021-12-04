using coffeeventureAPI.Model.unitsOfWork;
using coffeeventureAPI.Repository.Image;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageEntity = coffeeventureAPI.Data.Image;
namespace coffeeventureAPI.Service.Image
{
    public class ImageService: IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ImageService(IImageRepository imageRepository, IUnitOfWork unitOfWork) : base()
        {
            _imageRepository = imageRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ImageEntity>> Upload(IFormFileCollection files)
        {
            List<ImageEntity> result = new List<ImageEntity>();

            foreach (var file in files)
            {
                ImageEntity fileInfo = await _imageRepository.Upload(file);
                result.Add(fileInfo);
            }
            return result;

        }
    }
}
