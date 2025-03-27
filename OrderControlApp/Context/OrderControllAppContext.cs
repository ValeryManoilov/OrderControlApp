using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderControlApp.Models;

namespace OrderControlApp.Context
{
    public class OrderControllAppContext : IdentityDbContext<User>
    {
        public OrderControllAppContext(DbContextOptions<OrderControllAppContext> options)
            : base(options){}

        public DbSet<Client> Clients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Client>(options =>
            {
                options.Property(c => c.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Order>(options =>
            {
                options.Property(o => o.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Product>(options =>
            {
                options.Property(o => o.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<OrderProduct>(options =>
            {
                options.HasKey(o => new { o.OrderId, o.ProductId});
            });

            modelBuilder.Entity<Client>(options =>
            {
                options.HasMany(c => c.Orders)
                        .WithOne(o => o.Client)
                        .HasForeignKey(o => o.ClientId);
            });

            modelBuilder.Entity<Order>(options =>
            {
                options.HasMany(o => o.OrderProducts)
                        .WithOne(op => op.Order)
                        .HasForeignKey(o => o.OrderId);
            });

            modelBuilder.Entity<Product>(options =>
            {
                options.HasMany(p => p.OrderProducts)
                        .WithOne(op => op.Product)
                        .HasForeignKey(p => p.ProductId);
            });
        }

    }
}
