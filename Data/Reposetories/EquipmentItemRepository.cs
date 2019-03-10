using Data.Interfaces;
using Data.Models;

namespace Data.Reposetories
{
    public class EquipmentItemRepository : GenericRepository<StoreContext, EquipmentItem>, IEquipmentItemRepository
    {
        public EquipmentItemRepository(StoreContext context) : base(context)
        {
        }
    }
}
