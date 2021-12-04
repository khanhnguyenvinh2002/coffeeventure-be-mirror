
using coffeeventureAPI.Core.Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UserModel = coffeeventureAPI.Data.User;

namespace coffeeventureAPI.Data.UserDto
{
    public class UserDto : BaseModel
    {
        public UserDto() {}

        public UserDto(UserModel model) : base(model)
        {
            UserRole = model.UserRole?.Select(x => x.Role).Select(x => new RoleDto() { Id = x.Id, Name = x.Name });
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MidName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public short? Status { get; set; }
        public string AvatarPath { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public IEnumerable<RoleDto> UserRole { get; set; }
    }

    public class RoleDto
    {
        public RoleDto() {}
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
