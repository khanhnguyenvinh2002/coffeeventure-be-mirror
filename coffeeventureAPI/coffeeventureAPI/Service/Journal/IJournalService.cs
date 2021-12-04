using coffeeventureAPI.Dtos.Journal;
using coffeeventureAPI.Model;
using coffeeventureAPI.Repository.Journal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JournalEntity = coffeeventureAPI.Data.Journal;
using JournalLikeEntity = coffeeventureAPI.Data.JournalLike;
using ImageEntity = coffeeventureAPI.Data.Image;

namespace coffeeventureAPI.Service.Journal
{
    public interface IJournalService : IScoped
    {
         Task<JournalDto> AddJournal(JournalDto Journal);

         Task<bool> DeleteJournal(string id);
        Task<List<JournalDto>> GetAllJournals( JournalRequestDto request);
        Task<JournalDto> Merge(JournalDto dto);
        Task<List<JournalDto>> GetAllJournalsById();
        Task<IEnumerable<JournalDto>> Select(JournalRequestDto request);
        Task<int> Count(JournalRequestDto request);
        Task<List<ImageEntity>> Upload(IFormFileCollection files, string journalId);
        Task<JournalDto> GetJournalById(string id);
        Task<bool> Like(JournalDto dto);
        public Task<bool> UpdateJournal(JournalEntity Journal);
        Task<IEnumerable<JournalLikeEntity>> SelectUsersById(string id);
    }
}
