using Data.Interfaces;
using Data.Models;

namespace Data.Reposetories
{
    public class UserRepository : GenericRepository<StoreContext, User>, IUserRepository
    {
        public UserRepository(StoreContext context) : base(context)
        {
        }
    }
}
