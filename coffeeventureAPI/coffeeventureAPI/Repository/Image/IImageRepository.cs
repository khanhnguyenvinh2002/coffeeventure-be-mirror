using coffeeventureAPI.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageEntity = coffeeventureAPI.Data.Image;

namespace coffeeventureAPI.Repository.Image
{
    public interface IImageRepository : IScoped
    {
        Task<ImageEntity> Upload(IFormFile file);
    }
}
