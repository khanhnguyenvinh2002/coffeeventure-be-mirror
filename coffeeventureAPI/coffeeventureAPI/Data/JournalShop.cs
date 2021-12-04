using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace coffeeventureAPI.Data
{
    public partial class JournalShop
    {
        public string Id { get; set; }
        public string JournalId { get; set; }
        //[ForeignKey("JournalImage")]
        public string ShopId { get; set; }

        public virtual Journal Journal { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
