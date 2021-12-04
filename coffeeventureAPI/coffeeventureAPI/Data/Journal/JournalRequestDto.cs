using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coffeeventureAPI.Dtos.Base;

namespace coffeeventureAPI.Dtos.Journal
{
     public class JournalRequestDto : BaseRequestDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Feeling { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
