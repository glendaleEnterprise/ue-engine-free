using Glendale.Design.Dtos;
using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.User;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Glendale.Design.Services
{
    [RemoteService(false)]
    [Dependency(ReplaceServices = true)]
    [ExposeServices(
        typeof(IIdentityUserAppService),
        typeof(IdentityUserAppService),
        typeof(IUserAppService),
        typeof(UserAppService))]
    public class UserAppService : IdentityUserAppService, IUserAppService
    {
        private readonly ILogsService _logsService;
        private readonly IIdentityUserRepository _userRepository;

        public UserAppService(IdentityUserManager userManager, IIdentityUserRepository userRepository, IIdentityRoleRepository roleRepository
            , IOptions<IdentityOptions> identityOptions, ILogsService logsService) : base(userManager, userRepository, roleRepository, identityOptions)
        {
            _userRepository = userRepository;
            _logsService = logsService;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
        {
            if (CurrentUser.UserName != "admin" && CurrentUser.UserName != "sysadmin")
            {
                throw new UserFriendlyException("无权限");
            }
            // && p.Id!=CurrentUser.Id); //排除当前用户(满足创建项目选择人员使用)
            var result = await UserRepository.GetListAsync(p => p.UserName != "admin" && p.UserName != "guest");
            result = result.WhereIf(!input.Filter.IsNullOrWhiteSpace(), p => p.PhoneNumber.Contains(input.Filter)).ToList();
            var list = result.DistinctBy(p => new { p.Id }).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            var total = result.DistinctBy(p => new { p.Id }).Count();
            var data = ObjectMapper.Map<IEnumerable<Volo.Abp.Identity.IdentityUser>, IReadOnlyList<IdentityUserDto>>(list);
            foreach (var item in data)
            {
                var entitys = await UserRepository.GetOrganizationUnitsAsync(item.Id);
                item.ExtraProperties.AddOrUpdate("OrgIds", entitys.Select(x => x.DisplayName).ToArray());
            }
            return new PagedResultDto<IdentityUserDto>(total, data);
        }

        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public override async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
        {
            if (CurrentUser.UserName != "admin" && CurrentUser.UserName != "sysadmin")
            {
                throw new UserFriendlyException("无权限");
            }
            var listUser = await _userRepository.GetListAsync(p => p.PhoneNumber == input.PhoneNumber);
            if (listUser.Count > 0)
            {
                throw new UserFriendlyException(input.PhoneNumber + "已经存在，不能重复注册");
            }
            input.UserName = input.PhoneNumber;
            var entity = await base.CreateAsync(input);
            if (input.ExtraProperties.TryGetValue("OrgIds", out object orgIds))
            {
                await UserManager.SetOrganizationUnitsAsync(entity.Id, orgIds.ToString().FromJson<Guid[]>());
            }

            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "用户管理", Name = "添加", Content = $"添加了用户：[{input.PhoneNumber}]" });

            return entity;
        }

        /// <summary>
        /// 修改账号信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public override async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
        {
            //判断手机号是否已经绑定账户
            var listUser = await _userRepository.GetListAsync(p => p.PhoneNumber == input.PhoneNumber && p.Id != id);
            if (listUser.Count > 0)
            {
                throw new UserFriendlyException(input.PhoneNumber + "已经存在，不能重复关联账户");
            }
            if (input.UserName != "guest" && input.UserName != "admin" && input.UserName != "sysadmin") input.UserName = input.PhoneNumber;
            var entity = await base.UpdateAsync(id, input);
            if (input.ExtraProperties.TryGetValue("OrgIds", out object orgIds))
            {
                if (orgIds != null) await UserManager.SetOrganizationUnitsAsync(entity.Id, orgIds.ToString().FromJson<Guid[]>());
            }
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "用户管理", Name = "更新", Content = $"更新了用户：[{input.PhoneNumber}]" });
            return entity;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(Guid id)
        {
            //体验站没有删除用户功能
            return;
            //var entity = await UserManager.FindByIdAsync(id.ToString());
            //await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "用户管理", Name = "删除", Content = $"删除了用户：[{entity.PhoneNumber}]" });
            //await base.DeleteAsync(id);
        }
    }
}
