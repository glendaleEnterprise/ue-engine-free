using Glendale.Design.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Glendale.Design.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class DesignController : AbpController
    {
        protected DesignController()
        {
            LocalizationResource = typeof(DesignResource);
        }
    }
}