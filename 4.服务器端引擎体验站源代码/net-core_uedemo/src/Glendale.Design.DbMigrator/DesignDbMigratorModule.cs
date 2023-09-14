using Glendale.Design.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Glendale.Design.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(DesignEntityFrameworkCoreModule),
        typeof(DesignApplicationContractsModule)
        )]
    public class DesignDbMigratorModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            Volo.Abp.PermissionManagement.AbpPermissionManagementDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.SettingManagement.AbpSettingManagementDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.BackgroundJobs.BackgroundJobsDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.AuditLogging.AbpAuditLoggingDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.Identity.AbpIdentityDbProperties.DbTablePrefix = "Sys_";
            Volo.Abp.IdentityServer.AbpIdentityServerDbProperties.DbTablePrefix = "Ids_";
            Volo.Abp.FeatureManagement.FeatureManagementDbProperties.DbTablePrefix = "Sys_";
            base.PreConfigureServices(context);
        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
