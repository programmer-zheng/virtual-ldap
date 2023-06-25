using DingDingSync.Core.Configuration;
using DingDingSync.Core.Web;
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

            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DingDingSyncDbContextConfigurer.Configure(builder, configuration.GetConnectionString("Default"));

            return new DingDingSyncDbContext(builder.Options);
        }
    }
}
