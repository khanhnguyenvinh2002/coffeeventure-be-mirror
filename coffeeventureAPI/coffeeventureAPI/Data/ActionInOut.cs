using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class ActionInOut
    {
        public string Id { get; set; }
        public string ActionId { get; set; }
        public string InOutType { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
    }
}
