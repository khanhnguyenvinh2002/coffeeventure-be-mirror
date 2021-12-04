using coffeeventureAPI.Core.Repository;
using coffeeventureAPI.Model.unitsOfWork;
using System.Linq;
using System.Threading.Tasks;
using UserModel = coffeeventureAPI.Data.User;

namespace coffeeventureAPI.Repository.Account
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public AccountRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IQueryable<UserModel>> Select()
        {
            IQueryable<UserModel> query = _unitOfWork.Select<UserModel>();
            return await Task.FromResult(query);
        }

        #endregion Constructor
    }
}
