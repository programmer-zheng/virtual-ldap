using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace VirtualLdap.EntityFrameworkCore
{
    public static class VirtualLdapDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<VirtualLdapDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public static void Configure(DbContextOptionsBuilder<VirtualLdapDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection, ServerVersion.AutoDetect((MySqlConnection)connection));
        }
    }
}