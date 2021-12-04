using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Data
{
    public partial class Image
    {
        public Image()
        {
            //UserOrganization = new HashSet<UserOrganization>();
            //UserOrganization = new HashSet<UserOrganization>();
            ShopImage = new HashSet<ShopImage>(); 
            ReviewImage = new HashSet<ReviewImage>(); 
            JournalImage = new HashSet<JournalImage>();
        

    }

        public virtual ICollection<JournalImage> JournalImage { get; set; }
        public virtual ICollection<ReviewImage> ReviewImage { get; set; }
        public virtual ICollection<ShopImage> ShopImage { get; set; }
        public string Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public int? Size { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
