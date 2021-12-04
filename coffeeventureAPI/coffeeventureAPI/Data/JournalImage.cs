using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace coffeeventureAPI.Data
{
    public partial class JournalImage
    {
        public string Id { get; set; }
        public string JournalId { get; set; }
        //[ForeignKey("JournalImage")]
        public string ImageId { get; set; }

        public virtual Journal Journal { get; set; }
        public virtual Image Image { get; set; }
    }
}
