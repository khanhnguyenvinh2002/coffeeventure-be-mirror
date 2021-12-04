using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Data
{
    public partial class Journal
    {
        public Journal()
        {
            //UserOrganization = new HashSet<UserOrganization>();
            JournalImage = new HashSet<JournalImage>();
            JournalShop = new HashSet<JournalShop>();
            JournalLike = new HashSet<JournalLike>();
        }
        public virtual ICollection<JournalShop> JournalShop { get; set; }
        public virtual ICollection<JournalLike> JournalLike { get; set; }
        public virtual ICollection<JournalImage> JournalImage { get; set; }
        public string Id { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public string Feeling { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
}
