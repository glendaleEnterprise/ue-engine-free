using Glendale.Design.VirtualRoutes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShardingCore;
using ShardingCore.Bootstrapers;
using ShardingCore.TableExists;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace Glendale.Design.EntityFrameworkCore
{
    [DependsOn(
       typeof(DesignDomainModule),
       typeof(AbpEntityFrameworkCoreMySQLModule)
       )]
    public class DesignModelEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddAbpDbContext<DesignModelDbContext>(options =>
            {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure<DesignModelDbContext>(context =>
                {
                    //简洁
                    DIExtension.UseDefaultSharding<DesignModelDbContext>(context.ServiceProvider, context.DbContextOptions);

                    ////如果你只有单配置可以选择这个配置但是链接字符串必须和AddConfig内部一样
                    //context.DbContextOptions
                    //.UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion)
                    //.UseLoggerFactory(efLogger)
                    //.UseSharding<DesignDbContext>();
                });
            });
            //添加分表配置
            context.Services.AddShardingConfigure<DesignModelDbContext>()
               .AddEntityConfig(op =>
               {
                   //自动建表,如果您使用code-first建议选择false
                   op.CreateShardingTableOnStart = false;
                   //自动建库,如果您使用code-first建议修改为fsle
                   op.EnsureCreatedWithOutShardingTable = true;
                   //当无法获取路由时会返回默认值而不是报错
                   op.ThrowIfQueryRouteNotMatch = false;
                   op.UseShardingQuery((connectionString, builder) =>
                   {
                       builder.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion).UseLoggerFactory(efLogger);
                   });
                   op.UseShardingTransaction((connection, builder) =>
                   {
                       builder.UseMySql(connection, MySqlServerVersion.LatestSupportedServerVersion).UseLoggerFactory(efLogger);
                   });
                   op.AddShardingTableRoute<ModelTreeVirtualTableRoute>();
                   op.AddShardingTableRoute<ModelTypeVirtualTableRoute>();
                   op.AddShardingTableRoute<ModelPropertyVirtualTableRoute>();
               })
               .AddConfig(op =>
               {
                   op.ConfigId = "c1";
                   op.AddDefaultDataSource("ds0", configuration.GetConnectionString("Model"));
                   op.ReplaceTableEnsureManager(sp => new MySqlTableEnsureManager<DesignModelDbContext>());
               }).EnsureConfig();
        }

        //初始化完成
        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            base.OnPostApplicationInitialization(context);
            context.ServiceProvider.GetRequiredService<IShardingBootstrapper>().Start();
        }
        public static readonly ILoggerFactory efLogger = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
        });
    }
}
