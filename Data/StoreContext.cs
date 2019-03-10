using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data
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
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasMany<OrderItem>(m => m.OrderItems).WithOne(m => m.Order)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class StoreContextFactory : IDesignTimeDbContextFactory<StoreContext>
    {
        public StoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StoreContext>();
            optionsBuilder.UseSqlite("Data Source=Store.db");

            return new StoreContext(optionsBuilder.Options);
        }
    }
}

