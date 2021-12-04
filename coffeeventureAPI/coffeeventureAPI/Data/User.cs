using System;
using System.Collections.Generic;

namespace coffeeventureAPI.Data
{
    public partial class User
    {
        public User()
        {
            //UserOrganization = new HashSet<UserOrganization>();
            UserRole = new HashSet<UserRole>();
            UserShop = new HashSet<UserShop>();
        }
        public virtual ICollection<UserRole> UserRole { get; set; }
        public virtual ICollection<UserShop> UserShop { get; set; }

        public string Id { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MidName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public int? GroupId { get; set; }
        public short? Status { get; set; }
        public string RefreshToken { get; set; }
        public string Groups { get; set; }
        public string ReceiveEmail { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string OtherEmail { get; set; }
        public string Avatar { get; set; }
        public string Language { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string SyncSource { get; set; }
        public DateTime? SyncAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
}
