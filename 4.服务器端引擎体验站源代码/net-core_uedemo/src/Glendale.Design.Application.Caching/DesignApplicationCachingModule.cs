using Glendale.Design.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace Glendale.Design
{
    [DependsOn(
        typeof(DesignCoreModule),
        typeof(DesignApplicationContractsModule),
        typeof(AbpCachingStackExchangeRedisModule))]
    public class DesignApplicationCachingModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AbpDistributedCacheOptions>(options =>
            {
                // 统一命名
                options.KeyPrefix = CachePrefix.KeyPrefix;
                options.HideErrors = false;
                // 滑动过期30天
                options.GlobalCacheEntryOptions.SlidingExpiration = TimeSpan.FromDays(30);
                // 绝对过期60天
                options.GlobalCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
            });

            Configure<RedisCacheOptions>(options =>
            {
                options.Configuration = configuration["Redis:Configuration"];
            });

            var csredis = new CSRedis.CSRedisClient(configuration["Redis:Configuration"]);
            RedisHelper.Initialization(csredis);

            context.Services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
        }
    }
}