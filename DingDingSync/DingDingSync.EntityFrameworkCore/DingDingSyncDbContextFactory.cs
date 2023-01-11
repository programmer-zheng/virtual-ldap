using DingDingSync.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DingDingSync.EntityFrameworkCore
{
    public class DingDingSyncDbContextFactory : IDesignTimeDbContextFactory<DingDingSyncDbContext>
    {
        public DingDingSyncDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DingDingSyncDbContext>();

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(WebContentDirectoryFinder.CalculateContentRootFolder())
                .AddJsonFile("appsettings.json", optional: false);

            var configuration = configurationBuilder.Build();

            DingDingSyncDbContextConfigurer.Configure(builder, configuration.GetConnectionString("Default"));

            return new DingDingSyncDbContext(builder.Options);
        }
    }
}
