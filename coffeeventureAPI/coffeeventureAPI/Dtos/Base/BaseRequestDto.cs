using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Dtos.Base
{
    public abstract class BaseRequestDto
    {
        protected BaseRequestDto() { }

        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
