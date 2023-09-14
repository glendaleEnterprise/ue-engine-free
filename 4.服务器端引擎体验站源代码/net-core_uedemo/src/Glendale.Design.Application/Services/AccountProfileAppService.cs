using Glendale.Design.Dtos.Dictionary;
using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.User;
using Microsoft.AspNetCore.Authorization;
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
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Users;
using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 
    /// </summary>   
    [RemoteService(false)]
    [Dependency(ReplaceServices = true)]
    [ExposeServices(
        typeof(IProfileAppService),
        typeof(ProfileAppService),
        typeof(AccountProfileAppService)
        )]
    public class AccountProfileAppService : ProfileAppService, IProfileAppService
    {
        protected readonly IPermissionAppService PermissionAppService;
        protected readonly IDictionaryService DictionaryAppService;
        public AccountProfileAppService(IdentityUserManager userManager, IOptions<IdentityOptions> identityOptions, IPermissionAppService permissionAppService, IDictionaryService dictionaryAppService)
            : base(userManager, identityOptions)
        {
            PermissionAppService = permissionAppService;
            DictionaryAppService = dictionaryAppService;
        }
        /// <summary>
        /// 获取登录用户信息(角色、权限、菜单)
        /// </summary>
        /// <returns></returns>
        public override async Task<ProfileDto> GetAsync()
        {
            var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
            var dto = ObjectMapper.Map<Volo.Abp.Identity.IdentityUser, ProfileDto>(currentUser);
            var organizationUnits = await UserManager.GetOrganizationUnitsAsync(currentUser);
            var roles = await UserManager.GetRolesAsync(currentUser);
            var rolePermissions = new List<GetPermissionListResultDto>();
            if (dto.ExtraProperties.TryGetValue("position", out object position) && position != null && !string.IsNullOrEmpty(position.ToString()))
            {
                var positionDto = await DictionaryAppService.GetByValueAsync(position.ToString());
                dto.ExtraProperties.AddOrUpdate("position", positionDto.Name);
            }
            foreach (var role in roles)
            {
                var rolePermission = await PermissionAppService.GetAsync(RolePermissionValueProvider.ProviderName, role);
                rolePermission.Groups = rolePermission.Groups.Where(x => x.Name.StartsWith("Design.")).ToList();
                rolePermissions.Add(rolePermission);
            }
            dto.ExtraProperties.AddOrUpdate("organizationUnits", organizationUnits.Select(x => x.DisplayName));
            dto.ExtraProperties.AddOrUpdate("roles", rolePermissions);
            dto.ExtraProperties.AddOrUpdate("id", currentUser.Id);
            return dto;
        }

         


    }
}
