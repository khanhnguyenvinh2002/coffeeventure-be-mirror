using coffeeventureAPI.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserModel = coffeeventureAPI.Data.User;

namespace coffeeventureAPI.Repository.Account
{
    public interface IAccountRepository : IScoped
    {
        Task<IQueryable<UserModel>> Select();
    }
}
