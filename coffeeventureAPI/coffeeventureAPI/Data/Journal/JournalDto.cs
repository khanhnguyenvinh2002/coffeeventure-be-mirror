
using coffeeventureAPI.Core.Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JournalEntity = coffeeventureAPI.Data.Journal;

namespace coffeeventureAPI.Dtos.Journal
{
    public class JournalDto : BaseModel
    {
        public JournalDto()
        {

        }
        public JournalDto(JournalEntity entity) : base(entity)
        {
        }
        public string Id { get; set; }
        public string Content { get; set; }
        public IEnumerable<string> LikedUsers { get; set; }
        public int Likes { get; set; }
        public int Status { get; set; }
        public string UserName { get; set; }
        public string Feeling { get; set; }
        public IEnumerable<string> ImageDirectories { get; set; }
        public string AvatarPath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
