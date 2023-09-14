using Glendale.Design.Dtos;
using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.User;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Linq;
using DependencyAttribute = Volo.Abp.DependencyInjection.DependencyAttribute;

namespace Glendale.Design.Services
{
    [Authorize]
    public class SystemUserAppService : CrudAppService<IdentityUser, IdentityUserDto, IdentityUserDto, Guid, GetListIdentityUsersInput, IdentityUserCreateDto, IdentityUserUpdateDto>, IUserAppService
    {    
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly ILogsService LogsService;
        private readonly IIdentityUserRepository UserRepository;
        private readonly IdentityUserManager UserManager;

        public SystemUserAppService(IRepository<IdentityUser, Guid> repository, IIdentityUserRepository _UserRepository, ILogsService _LogsService, IdentityUserManager _UserManager) : base(repository)
        {
            UserRepository = _UserRepository;
            LogsService = _LogsService;
            UserManager = _UserManager;
        }         
          
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetListIdentityUsersInput input)
        {
            //&& p.UserName != "guest"  //guest用户允许被选择，被 查询
            var Query = Repository.Where(p => p.UserName != "admin")// && p.Id!=CurrentUser.Id)
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), p => p.PhoneNumber.Contains(input.Filter))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), p => p.Name.Contains(input.Name));

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);

            var dtos = ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(entitys);

            foreach (var item in dtos)
            {
                var orglist = await UserRepository.GetOrganizationUnitsAsync(item.Id);
                item.ExtraProperties.AddOrUpdate("OrgIds", orglist.Select(x => x.DisplayName).ToArray());
                var userlist = await UserRepository.GetRoleNamesAsync(item.Id);
                item.ExtraProperties.AddOrUpdate("RoleNames", userlist.ToArray());
            }

            return new PagedResultDto<IdentityUserDto>(totalCount, dtos);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<IdentityUserDto>> GetUsersListAsync(GetListIdentityUsersInput input)
        {
            var Query = Repository.Where(p => p.UserName != "admin")
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), p => p.PhoneNumber.Contains(input.Filter))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), p => p.Name.Contains(input.Name));

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);

            var dtos = ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(entitys);

            foreach (var item in dtos)
            {
                var orglist = await UserRepository.GetOrganizationUnitsAsync(item.Id);
                item.ExtraProperties.AddOrUpdate("OrgIds", orglist.Select(x => x.DisplayName).ToArray());
                var userlist = await UserRepository.GetRoleNamesAsync(item.Id);
                item.ExtraProperties.AddOrUpdate("RoleNames", userlist.ToArray());
            }

            return new PagedResultDto<IdentityUserDto>(totalCount, dtos);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInfoAsync(Guid id, UserDto input)
        {
            if (input.UserName == "guest")
            {
                throw new UserFriendlyException(input.PhoneNumber + "不能修改 guest");
            }
            if (!string.IsNullOrEmpty(input.PhoneNumber))
            {
                //判断手机号是否已经绑定账户
                var listUser = await Repository.Where(p => p.PhoneNumber == input.PhoneNumber && p.Id != id).CountAsync();
                if (listUser > 0)
                {
                    throw new UserFriendlyException(input.PhoneNumber + "已经存在，不能重复关联账户");
                }
            }

            try
            {
                var user = await Repository.GetAsync(id);
                if (!string.IsNullOrEmpty(input.PhoneNumber))
                {
                    await UserManager.SetUserNameAsync(user, input.PhoneNumber);
                    await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber);
                }
                user.Name = input.Name;
                user.ExtraProperties.AddOrUpdate("Describe", input.Describe);
                var entity = Repository.UpdateAsync(user);

                await LogsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "用户信息", Name = "更新", Content = $"用户信息被修改[{input.PhoneNumber}]" });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
