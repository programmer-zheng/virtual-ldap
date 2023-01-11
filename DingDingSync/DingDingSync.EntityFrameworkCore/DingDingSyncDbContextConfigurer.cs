using Microsoft.EntityFrameworkCore;

namespace DingDingSync.EntityFrameworkCore
{
    public static class DingDingSyncDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<DingDingSyncDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 7)));
        }
    }
}
