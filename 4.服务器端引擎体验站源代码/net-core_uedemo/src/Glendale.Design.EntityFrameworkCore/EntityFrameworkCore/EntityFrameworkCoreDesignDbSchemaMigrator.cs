using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Glendale.Design.Data;
using Volo.Abp.DependencyInjection;

namespace Glendale.Design.EntityFrameworkCore
{
    public class EntityFrameworkCoreDesignDbSchemaMigrator
        : IDesignDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreDesignDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the DesignDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<DesignDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
