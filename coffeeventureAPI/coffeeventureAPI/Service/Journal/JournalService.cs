using coffeeventureAPI.Dtos.Journal;
using coffeeventureAPI.Model;
using coffeeventureAPI.Model.unitsOfWork;
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
    public class JournalService : IJournalService, IScoped
    {
        private readonly IJournalRepository _journalRepository;
        private readonly IUnitOfWork _unitOfWork;
        public JournalService(IJournalRepository JournalRepository, IUnitOfWork unitOfWork)
        {
            _journalRepository = JournalRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<JournalDto> AddJournal(JournalDto Journal)
        {
            return await _journalRepository.AddJournal(Journal);
        }

        public async Task<bool> DeleteJournal(string id)
        {
           return await _journalRepository.DeleteJournal(id);
        }

        public async Task<List<JournalDto>> GetAllJournals( JournalRequestDto request)
        {
            return await _journalRepository.GetAllJournals(request);
        }
        public async Task<List<JournalDto>> GetAllJournalsById()
        {
            return await _journalRepository.GetAllJournalsById();
        }
        public async Task<JournalDto> Merge(JournalDto dto)
        {
            // Begin transaction
            using var transaction = _unitOfWork.BeginTransaction();

            // Merge Shop
            var dtoResult = await _journalRepository.Merge(dto);

            // Commit transaction
            transaction.Commit();
            return await _journalRepository.GetJournalById(dto.Id);
        }
        public async Task<List<ImageEntity>> Upload(IFormFileCollection files, string journalId)
        {

            List<ImageEntity> result = new List<ImageEntity>();

            foreach (var file in files)
            {
                ImageEntity fileInfo = await _journalRepository.Upload(file, journalId);
                result.Add(fileInfo);
            }
            return result;
        }
        //[RedisCache(Duration = 1, Measure = TimeMeasure.Day)]
        public async Task<IEnumerable<JournalDto>> Select( JournalRequestDto request)
        {
            return await _journalRepository.Select(request);
        }
        public async Task<int> Count( JournalRequestDto request)
        {
            return await _journalRepository.Count(request);
        }
        public async Task<JournalDto> GetJournalById(string id)
        {
            return await _journalRepository.GetJournalById(id);
        }
        public async Task<bool> Like(JournalDto dto)
        {
            return await _journalRepository.Like(dto);
        }
        public async Task<bool> UpdateJournal(JournalEntity Journal)
        {
            return await _journalRepository.UpdateJournal(Journal);
        }

        public async Task<IEnumerable<JournalLikeEntity>> SelectUsersById(string id)
        {
            return await _journalRepository.SelectUsersById(id);
        }
        
    }
}
