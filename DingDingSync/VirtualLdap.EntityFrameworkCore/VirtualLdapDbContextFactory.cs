using VirtualLdap.Core.Configuration;
using VirtualLdap.Core.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VirtualLdap.EntityFrameworkCore
{
    public class VirtualLdapDbContextFactory : IDesignTimeDbContextFactory<VirtualLdapDbContext>
    {
        public VirtualLdapDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<VirtualLdapDbContext>();

            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            VirtualLdapDbContextConfigurer.Configure(builder, configuration.GetConnectionString("Default"));

            return new VirtualLdapDbContext(builder.Options);
        }
    }
}
