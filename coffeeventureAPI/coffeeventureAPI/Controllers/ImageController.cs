//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using coffeeventureAPI.Data;
using ImageEntity = coffeeventureAPI.Data.Image;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using coffeeventureAPI.Service.Image;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using coffeeventureAPI.Model.unitsOfWork;

namespace coffeeventureAPI.Controllers
{
    [Route("image")]
    [Authorize]
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;
        public ImageController( IImageService imageService, IUnitOfWork unitOfWork)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<List<ImageEntity>> Upload()
        {
            //if (Request.Headers["id-token"].Count() > 0)
            //{
            //    var stream = Request.Headers["id-token"];
            //    var handler = new JwtSecurityTokenHandler();
            //    var jsonToken = handler.ReadToken(stream);
            //    var tokenS = jsonToken as JwtSecurityToken;
            //    var jti = tokenS.Claims.First(claim => claim.Type == "jti").Value;
            //    _unitOfWork.SetCurrentUserId(jti);
            //}
            return await _imageService.Upload(Request.Form.Files);
        }
        //}
        //public async Task<bool> Upload()
        //{
        //    try
        //    {
        //        var files = Request.Form.Files;
        //        string folderName = "Upload";
        //        string webRootPath = _hostingEnvironment.WebRootPath;
        //        string newPath = Path.Combine(webRootPath, folderName);
        //        if (!Directory.Exists(newPath))
        //        {
        //            Directory.CreateDirectory(newPath);
        //        }
        //        //var folderName = Path.Combine("Resources", "Images");
        //        //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        if (files.Any(f => f.Length == 0))
        //        {
        //            return await Task.FromResult(false);
        //        }
        //        foreach (var file in files)
        //        {
        //            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //            string fullPath = Path.Combine(newPath, fileName);
        //            await _imageService.Upload(fullPath);
        //            //var fullPath = Path.Combine(pathToSave, fileName);
        //            //var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //        }
        //        return await Task.FromResult(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(false);
        //    }
        //}

        //[HttpPost, DisableRequestSizeLimit]
        //public ObjectResult UploadFile()
        //{
        //    try
        //    {
        //        var file = Request.Form.Files[0];
        //        string folderName = "Upload";
        //        string webRootPath = _hostingEnvironment.WebRootPath;
        //        string newPath = Path.Combine(webRootPath, folderName);
        //        if (!Directory.Exists(newPath))
        //        {
        //            Directory.CreateDirectory(newPath);
        //        }
        //        string fileName = "";
        //        if (file.Length > 0)
        //        {
        //            fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //            string fullPath = Path.Combine(newPath, fileName);
        //            await _imageService.Upload(fullPath);
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //        }

        //        return Ok(fileName);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> Upload()
        //{
        //    try
        //    {
        //        var files = Request.Form.Files;
        //        var folderName = Path.Combine("Resources", "Images");
        //        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        if (files.Any(f => f.Length == 0))
        //        {
        //            return BadRequest();
        //        }
        //        foreach (var file in files)
        //        {
        //            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //            var fullPath = Path.Combine(pathToSave, fileName);
        //            var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //        }
        //        return Ok("All the files are successfully uploaded.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
    }
}
  

        //[HttpPost]
        //public async Task<bool> Upload()
        //{
        //    try
        //    {
        //        var files = Request.Form.Files[0];
        //        string folderName = "Upload";
        //        string webRootPath = _hostingEnvironment.WebRootPath;
        //        string newPath = Path.Combine(webRootPath, folderName);
        //        if (!Directory.Exists(newPath))
        //        {
        //            Directory.CreateDirectory(newPath);
        //        }
        //        //var folderName = Path.Combine("Resources", "Images");
        //        //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        if (files.Any(f => f.Length == 0))
        //        {
        //            return await Task.FromResult(false);
        //        }
        //        foreach (var file in files)
        //        {
        //            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //            string fullPath = Path.Combine(newPath, fileName);
        //            await _imageService.Upload(fullPath);
        //            //var fullPath = Path.Combine(pathToSave, fileName);
        //            //var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //        }
        //        return await Task.FromResult(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(false);
        //    }
        //}
