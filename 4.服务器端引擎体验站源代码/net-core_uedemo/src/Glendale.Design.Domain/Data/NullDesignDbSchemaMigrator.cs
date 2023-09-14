using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Glendale.Design.Data
{
    /* This is used if database provider does't define
     * IDesignDbSchemaMigrator implementation.
     */
    public class NullDesignDbSchemaMigrator : IDesignDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}