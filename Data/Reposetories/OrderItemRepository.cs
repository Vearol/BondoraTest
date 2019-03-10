using Data.Interfaces;
using Data.Models;

namespace Data.Reposetories
{
    public class OrderItemRepository : GenericRepository<StoreContext, OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(StoreContext context) : base(context)
        {
        }
    }
}
