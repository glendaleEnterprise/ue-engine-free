using System.Collections.Generic;
using System.IO;
using Glendale.Design.VirtualRoutes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;
using ShardingCore.Bootstrapers;

namespace Glendale.Design.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database and Script-Migration commands ) 
     * Add-Migration  -Context DesignDbContext
     * Update-Database  -Context DesignDbContext
     * Remove-Migration lfl0619 -Context DesignDbContext
     * Script-Migration -From lfl0701 -To lfl0807 -Context DesignDbContext
     */
    public class DesignDbContextFactory : IDesignTimeDbContextFactory<DesignDbContext>
    {
        public DesignDbContext CreateDbContext(string[] args)
        {
            DesignEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<DesignDbContext>()
                .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);

            return new DesignDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Glendale.Design.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
