using EcommerceApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ItemData> ItemDatas { get; set; }

    }
}
