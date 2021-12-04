//using coffeeventureAPI.Core.Base.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//using UserEntity = coffeeventureAPI.Data.User;
//namespace coffeeventureAPI.Data
//{
//    public class Account
//    {
//        public bool IsAuthenticated { get; set; }
//        public bool CanAccess { get; set; }
//        public AccountInfo User { get; set; }
//        public string Redirect { get; set; }
//    }

//    public class AccountInfo : BaseModel
//    {
//        public string Id { get; set; }
//        public string FirstName { get; set; }
//        public string UserName { get; set; }
//        public string FullName { get; set; }
//        public string Avatar { get; set; }
//        public string Language { get; set; }

//        public AccountInfo(UserEntity user) : base(user)
//        {
//        }
//    }
//}
