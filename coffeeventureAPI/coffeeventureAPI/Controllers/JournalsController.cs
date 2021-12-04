using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coffeeventureAPI.Data;
using coffeeventureAPI.Service.Journal;
using JournalEntity = coffeeventureAPI.Data.Journal;
using JournalLikeEntity = coffeeventureAPI.Data.JournalLike;
using ImageEntity = coffeeventureAPI.Data.Image;
using coffeeventureAPI.Dtos.Journal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace coffeeventureAPI.Controllers
{
    [Route("journal")]
    public class JournalsController : Controller
    {
        private readonly IJournalService _journalService;

        public JournalsController(IJournalService journalService)
        {
            _journalService = journalService;
        }

        [Route("get-all")]
        [HttpGet]
        public async Task<IEnumerable<JournalDto>> GetAllJournals([FromQuery] JournalRequestDto request)
        {
            return await _journalService.GetAllJournals(request);
        }
        [Route("get-all-by-id")]
        [HttpGet]
        public async Task<IEnumerable<JournalDto>> GetAllJournalsById()
        {
            return await _journalService.GetAllJournalsById();
        }

        [HttpGet]
        //[RedisCache(Duration = 1, Measure = TimeMeasure.Day)]
        public async Task<IEnumerable<JournalDto>> Select([FromQuery]JournalRequestDto request)
        {
            return await _journalService.Select(request);
        }
        [Route("count")]
        [HttpGet]
        public async Task<int> Count([FromQuery] JournalRequestDto request)
        {
            return await _journalService.Count(request);
        }
        [Route("{id}")]
        [HttpDelete]
        public async Task<bool> Delete([FromRoute] string id)
        {
            return await _journalService.DeleteJournal(id);
        }

        [HttpPost]
        public Task<JournalDto> AddJournal([FromBody] JournalDto entity)
        {
            return _journalService.AddJournal(entity);
        }

        [Route("{id}")]
        [HttpGet]
        public Task<JournalDto> GetJournalById([FromRoute] string id)
        {
            return _journalService.GetJournalById(id);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<bool> UpdateJournal([FromBody] JournalEntity entity)
        {
            return await _journalService.UpdateJournal(entity);
        }
        [Route("merge")]
        [HttpPost]
        public async Task<JournalDto> Merge([FromQuery] JournalDto dto)
        {
            return await _journalService.Merge(dto);
        }
        //public async Task<JournalDto> Merge()
        //{
        //    var file = Request.Form.Files;
        //    var stream = Request.Headers["body"];
        //    string json = JsonConvert.SerializeObject(stream);
        //    JObject juser = JObject.Parse(stream);
        //    JournalDto requestDto = new JournalDto();
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

        //    return await _journalService.Merge(requestDto, file);
        //}
        [Route("upload-images/{id}")]
        [HttpPost]
        public async  Task<List<ImageEntity>> Upload([FromRoute]string id)
        {
            var file = Request.Form.Files;
            return await _journalService.Upload(file, id);
        }
        [Route("like")]
        [HttpPost]
        public async Task<bool> Like([FromBody] JournalDto dto)
        {
            return await _journalService.Like(dto);
        }

        [Route("journal-like/{id}")]
        [HttpGet]
        //[RedisCache(Duration = 1, Measure = TimeMeasure.Day)]
        public async Task<IEnumerable<JournalLikeEntity>> SelectUsersById([FromRoute] string id)
        {
            return await _journalService.SelectUsersById(id);
        }
    }
}
