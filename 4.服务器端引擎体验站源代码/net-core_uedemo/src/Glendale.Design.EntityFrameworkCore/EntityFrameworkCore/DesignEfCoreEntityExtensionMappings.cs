using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Glendale.Design.EntityFrameworkCore
{
    public static class DesignEfCoreEntityExtensionMappings
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

        public static void Configure()
        {
            DesignGlobalFeatureConfigurator.Configure();
            DesignModuleExtensionConfigurator.Configure();

            OneTimeRunner.Run(() =>
            {
                ObjectExtensionManager.Instance
                    //.MapEfCoreProperty<IdentityUser, string>("Avatar", (entityBuilder, propertyBuilder) => { propertyBuilder.IsRequired(false).HasMaxLength(32); })

                    .MapEfCoreProperty<IdentityUser, DateTime?>("ValidityDate")
                    .MapEfCoreProperty<IdentityUser, string>("Position", (entityBuilder, propertyBuilder) => { propertyBuilder.IsRequired(false).HasMaxLength(10); })
                    .MapEfCoreProperty<IdentityUser, string>("Describe", (entityBuilder, propertyBuilder) => { propertyBuilder.IsRequired(false).HasMaxLength(500); })
                    .MapEfCoreProperty<IdentityUser, string>("CorpName", (entityBuilder, propertyBuilder) => { propertyBuilder.IsRequired(false).HasMaxLength(200); })
                    .MapEfCoreProperty<IdentityRole, string>("Describe", (entityBuilder, propertyBuilder) => { propertyBuilder.HasMaxLength(20); })
                    .MapEfCoreProperty<IdentityRole, string>("RoleType", (entityBuilder, propertyBuilder) => { propertyBuilder.HasMaxLength(20); })
                    .MapEfCoreProperty<OrganizationUnit, bool>("System", (entityBuilder, propertyBuilder) => { propertyBuilder.HasDefaultValue(false); })
                    .MapEfCoreProperty<OrganizationUnit, string>("Describe", (entityBuilder, propertyBuilder) => { propertyBuilder.IsRequired(false).HasMaxLength(500); });
            });
        }
    }
}
