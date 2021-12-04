using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageEntity = coffeeventureAPI.Data.Image;

namespace coffeeventureAPI.Service.Image
{
    public interface IImageService
    {
        public Task<List<ImageEntity>> Upload(IFormFileCollection files);
    }
}
