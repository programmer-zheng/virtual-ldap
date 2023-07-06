using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace VirtualLdap.EntityFrameworkCore
{
    public static class VirtualLdapDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<VirtualLdapDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
#if DEBUG
                .LogTo(Console.WriteLine, LogLevel.Information)
#endif
                ;
        }

        public static void Configure(DbContextOptionsBuilder<VirtualLdapDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection, ServerVersion.AutoDetect((MySqlConnection)connection))
#if DEBUG
                .LogTo(Console.WriteLine, LogLevel.Information)
#endif
                ;
        }
    }
}