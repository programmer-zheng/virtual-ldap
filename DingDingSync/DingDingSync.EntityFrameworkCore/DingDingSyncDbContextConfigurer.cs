using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data.Common;

namespace DingDingSync.EntityFrameworkCore
{
    public static class DingDingSyncDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<DingDingSyncDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public static void Configure(DbContextOptionsBuilder<DingDingSyncDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection, ServerVersion.AutoDetect((MySqlConnection)connection));
        }
    }
}