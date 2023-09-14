using Volo.Abp.Modularity;
using Volo.Abp.EntityFrameworkCore;

namespace Glendale.Design
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class DesignCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //context.Services.AddHttpClient<ISevenZipCompressor, SevenZipCompressor>();
        }
    }
}
