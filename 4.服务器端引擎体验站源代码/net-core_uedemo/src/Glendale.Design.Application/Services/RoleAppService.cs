using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Glendale.Design.Services
{
    [RemoteService(false)]
    [Dependency(ReplaceServices = true)]
    [ExposeServices(
        typeof(IIdentityRoleAppService),
        typeof(IdentityRoleAppService),
        typeof(IRoleAppService),
        typeof(RoleAppService))]
    public class RoleAppService : IdentityRoleAppService, IRoleAppService
    {
        protected readonly OrganizationUnitManager OrganizationUnitManager;
        public RoleAppService(IdentityRoleManager roleManager, IIdentityRoleRepository roleRepository, OrganizationUnitManager organizationUnitManager) : base(roleManager, roleRepository)
        {
            OrganizationUnitManager = organizationUnitManager;
        }
        public override async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
        {
            var entity = await base.CreateAsync(input);
            //if (input.ExtraProperties.TryGetValue("OrgId", out object orgIds))
            //{
            //    await OrganizationUnitManager.AddRoleToOrganizationUnitAsync(entity.Id, orgIds.To<Guid>());
            //}
            return entity;
        }
        public override async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
        {
            var entity = await base.UpdateAsync(id, input);
            //if (input.ExtraProperties.TryGetValue("OrgId", out object orgIds))
            //{
            //    await OrganizationUnitManager.AddRoleToOrganizationUnitAsync(entity.Id, orgIds.To<Guid>());
            //}
            return entity;
        }
    }
}
