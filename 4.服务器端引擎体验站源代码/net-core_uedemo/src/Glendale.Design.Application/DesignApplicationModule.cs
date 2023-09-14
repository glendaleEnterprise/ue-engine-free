using AutoMapper.Configuration;
using Glendale.Design.Dtos;
using Glendale.Design.Dtos.File;
using Glendale.Design.Jobs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace Glendale.Design
{
    [DependsOn(
        typeof(DesignDomainModule),
        typeof(DesignCoreModule),
        //typeof(DesignApplicationCachingModule),
        typeof(DesignApplicationContractsModule),
        typeof(AbpAccountApplicationModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule),
        typeof(AbpBackgroundWorkersModule)
        )]
    public class DesignApplicationModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context.AddBackgroundWorker<ModelStateCheckerWorker>();
            context.AddBackgroundWorker<CADStateCheckerWorker>();
        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
           
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<DesignApplicationModule>();
            });
            var configuration = context.Services.GetConfiguration();
            Configure<ModelFileOption>(options =>
            {
                options.Url = configuration["ModelFile:Url"];
                options.Token = configuration["ModelFile:Token"];
            });
            Configure<ModelDBOptions>(options =>
            {
                var dbstr = configuration["ConnectionStrings:Model"];

                var matches = new System.Text.RegularExpressions.Regex("Database=(?<Database>\\w+)").Match(dbstr).Groups;
                options.Database = matches["Database"].Value;
            });
           
            Configure<AbpBackgroundJobWorkerOptions>(options =>//启用会执行不了job
            {
                options.DefaultTimeout = 30;//超时时间30分钟，重试次数5次
            });
            Configure<AbpBackgroundJobOptions>(options => { options.IsJobExecutionEnabled = true; });
        }
    }
}
