using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Core.API.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("Unauthorized access.")
        {
        }
    }
}
