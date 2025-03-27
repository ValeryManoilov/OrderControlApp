using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderControlApp.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderControllAppContext>
    {
        public OrderControllAppContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<OrderControllAppContext>();
            var connectionString = configuration.GetConnectionString("postgresString");

            builder.UseNpgsql(connectionString, options =>
            {
                options.MigrationsAssembly(typeof(OrderControllAppContext).Assembly.FullName);
            });

            return new OrderControllAppContext(builder.Options);
        }
    }
}