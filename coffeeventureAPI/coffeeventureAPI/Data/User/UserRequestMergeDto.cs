using System;
using System.Collections.Generic;
using System.Text;

namespace coffeeventureAPI.Data.UserDto
{
    public class UserRequestMergeDto
    {
        public bool IsUpdUserOrg { get; set; }
        public string[] OrgIds { get; set; }
    }
}
