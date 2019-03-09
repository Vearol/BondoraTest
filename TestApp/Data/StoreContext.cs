using Microsoft.EntityFrameworkCore;
using TestApp.Data.Models;

namespace TestApp.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<EquipmentItem> EquipmentItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
