using System;

namespace coffeeventureAPI.Data
{
    public partial class JournalLike
    {
        public string Id { get; set; }
        public string JournalId { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Journal Journal { get; set; }
    }
}
