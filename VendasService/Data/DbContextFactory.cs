using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VendasService.Data
{
    public class VendasContextFactory : IDesignTimeDbContextFactory<VendasContext>
    {
        public VendasContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<VendasContext>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 43)));

            return new VendasContext(optionsBuilder.Options);
        }
    }
}