using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Glendale.Design
{
    [Dependency(ReplaceServices = true)]
    public class DesignBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "Design";
    }
}
