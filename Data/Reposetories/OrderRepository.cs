using Data.Interfaces;
using Data.Models;

namespace Data.Reposetories
{
    public class OrderRepository : GenericRepository<StoreContext, Order>, IOrderRepository
    {
        public OrderRepository(StoreContext context) : base(context)
        {
        }
    }
}
