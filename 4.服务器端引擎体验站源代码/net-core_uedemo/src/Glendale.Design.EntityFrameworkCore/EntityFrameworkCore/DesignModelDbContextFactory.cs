using System;
using System.Collections.Generic;
using System.IO;
using Glendale.Design.VirtualRoutes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    * (like Add-Migration and Update-Database and Script-Migration commands ) */
    public class DesignModelDbContextFactory : IDesignTimeDbContextFactory<DesignModelDbContext>
    {
        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Glendale.Design.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
        static DesignModelDbContextFactory()
        {
            DesignEfCoreEntityExtensionMappings.Configure();

            var services = new ServiceCollection();
            var configuration = BuildConfiguration();
            System.Console.WriteLine("=======================================================================");
            System.Console.WriteLine(configuration.GetConnectionString("Model"));
            if (configuration.GetConnectionString("Model").IsNotNullOrEmpty())
            {
                string assemblyName = typeof(DesignModelDbContext).Namespace;
                services.AddShardingDbContext<DesignModelDbContext>()
                    .AddEntityConfig(op =>
                    {
                        op.CreateShardingTableOnStart = false;
                        op.EnsureCreatedWithOutShardingTable = false;
                        op.UseShardingQuery((connection, o) =>
                         o.UseMySql(connection, MySqlServerVersion.LatestSupportedServerVersion, x => x.MigrationsAssembly(assemblyName))
                          .ReplaceService<IMigrationsSqlGenerator, ShardingMySqlMigrationsSqlGenerator<DesignModelDbContext>>());
                        op.UseShardingTransaction((connection, builder) => builder.UseMySql(connection, MySqlServerVersion.LatestSupportedServerVersion));
                        op.AddShardingTableRoute<ModelTreeVirtualTableRoute>();
                        op.AddShardingTableRoute<ModelTypeVirtualTableRoute>();
                        op.AddShardingTableRoute<ModelSpaceVirtualTableRoute>();
                        op.AddShardingTableRoute<ModelPropertyVirtualTableRoute>();
                        op.AddShardingTableRoute<ModelPropertySpaceVirtualTableRoute>();
                        op.AddShardingTableRoute<ModelDrawingVirtualTableRoute>();
                        op.AddShardingTableRoute<ModelDrawingRvtIdVirtualTableRoute>();
                    })
                    .AddConfig(op =>
                    {
                        op.ConfigId = "c1";
                        op.AddDefaultDataSource("ds0", configuration.GetConnectionString("Model"));
                    }).EnsureConfig();
                services.AddLogging();
                var buildServiceProvider = services.BuildServiceProvider();
                ShardingContainer.SetServices(buildServiceProvider);
                ShardingContainer.GetService<IShardingBootstrapper>().Start();
            }
        }

        public DesignModelDbContext CreateDbContext(string[] args)
        {
            return ShardingContainer.GetService<DesignModelDbContext>();
        }
    }
}
