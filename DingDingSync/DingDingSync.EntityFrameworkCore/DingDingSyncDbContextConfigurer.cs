using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
