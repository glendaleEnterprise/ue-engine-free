using Glendale.Design.Dtos;
using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.Role;
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
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    [Authorize]
    public class SystemRoleAppService : CrudAppService<IdentityRole, IdentityRoleDto, IdentityRoleDto, Guid, GetListIdentityRolesInput, IdentityRoleCreateDto, IdentityRoleUpdateDto>, IRoleAppService
    {    
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly ILogsService LogsService;
        private readonly IIdentityRoleRepository RoleRepository;
        private readonly IdentityRoleManager RoleManager;

        public SystemRoleAppService(IRepository<IdentityRole, Guid> repository, IIdentityRoleRepository _RoleRepository, ILogsService _LogsService, IdentityRoleManager _RoleManager) : base(repository)
        {
            RoleRepository = _RoleRepository;
            LogsService = _LogsService;
            RoleManager = _RoleManager;
        }         
          
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<IdentityRoleDto>> GetAllListAsync(GetListIdentityRolesInput input)
        {
            var entitys = await Repository
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), p => p.Name.Contains(input.Name)).ToListAsync();

            var list = ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(entitys);

            //foreach (var item in dtos)
            //{
            //    item.ExtraProperties.AddOrUpdate("RoleType", orglist.Select(x => x.DisplayName).ToArray());
            //}
            if (!string.IsNullOrEmpty(input.RoleType))
            {
                list = list.Where(s => s.ExtraProperties.Where(t => t.Key == "RoleType" && t.Value != null && t.Value.ToString() == input.RoleType).Count() > 0).ToList();
            }
            return list;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> CreateInfoAsync(RoleDto input)
        {
            try
            {
                var role = new IdentityRole(Guid.NewGuid(), input.Name);
                role.ExtraProperties.AddOrUpdate("RoleType", input.RoleType);
                role.ExtraProperties.AddOrUpdate("Describe", input.Describe);
                var entity = Repository.InsertAsync(role);

                await LogsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "角色信息", Name = "添加", Content = $"角色信息被添加[{input.Name}]" });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> UpdateInfoAsync(Guid id, RoleDto input)
        {
            try
            {
                var role = await Repository.GetAsync(id);
                role.ChangeName(input.Name);
                if(!string.IsNullOrEmpty(input.RoleType))
                    role.ExtraProperties.AddOrUpdate("RoleType", input.RoleType);
                if(!string.IsNullOrEmpty(input.Describe))
                    role.ExtraProperties.AddOrUpdate("Describe", input.Describe);
                var entity = Repository.UpdateAsync(role);

                await LogsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "角色信息", Name = "更新", Content = $"角色信息被修改[{input.Name}]" });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
