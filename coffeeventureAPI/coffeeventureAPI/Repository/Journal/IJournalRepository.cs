using coffeeventureAPI.Dtos.Journal;
using coffeeventureAPI.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JournalEntity = coffeeventureAPI.Data.Journal;
using JournalLikeEntity = coffeeventureAPI.Data.JournalLike;
using ImageEntity = coffeeventureAPI.Data.Image;

namespace coffeeventureAPI.Repository.Journal
{
    public interface IJournalRepository : IScoped
    {
        Task<List<JournalDto>> GetAllJournals( JournalRequestDto request);
        Task<List<JournalDto>> GetAllJournalsById();
        Task<JournalDto> Merge(JournalDto model);
        Task<ImageEntity> Upload(IFormFile file, string shopId);
        Task<IEnumerable<JournalDto>> Select(JournalRequestDto request);
        Task<int> Count(JournalRequestDto request);
        Task<JournalDto> GetJournalById(string id);
        Task<bool> Like(JournalDto dto);
        Task<JournalDto> AddJournal(JournalDto journal);
        Task<bool> UpdateJournal(JournalEntity journal);
        Task<bool> DeleteJournal(string id);
        Task<IEnumerable<JournalLikeEntity>> SelectUsersById(string id);
    }
}
