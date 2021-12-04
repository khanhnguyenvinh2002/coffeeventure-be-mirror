using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class Action
    {
        public string Id { get; set; }
        public string RoutePath { get; set; }
        public string Method { get; set; }
        public string Tag { get; set; }
    }
}
