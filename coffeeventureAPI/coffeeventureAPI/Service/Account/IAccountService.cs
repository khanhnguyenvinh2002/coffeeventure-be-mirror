using coffeeventureAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static coffeeventureAPI.Controllers.AccountController;

namespace coffeeventureAPI.Service.Account
{
    public interface IAccountService : IScoped
    {
        Task<AccountChildDto> LoginChild(LoginRequestDto request);
        Task<AccountChildDto> Register(LoginRequestDto request);
    }

}
