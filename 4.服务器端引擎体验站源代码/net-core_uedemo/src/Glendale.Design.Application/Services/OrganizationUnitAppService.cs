using Glendale.Design.Dtos;
using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Linq;
using Volo.Abp.ObjectExtending;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 组织机构
    /// </summary>
    [Authorize]
    public class OrganizationUnitAppService : CrudAppService<OrganizationUnit, OrganizationUnitDto, OrganizationUnitListDto, Guid, GetListOrganizationUnitInput,OrganizationUnitCreateDto, OrganizationUnitUpdateDto>, IOrganizationUnitAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IIdentityUserAppService UserAppService;
        private readonly IIdentityRoleAppService RoleAppService;
        private readonly OrganizationUnitManager OrganizationUnitManager;
        private readonly IIdentityUserRepository UserRepository;
        private readonly IOrganizationUnitRepository OrganizationUnitRepository;       
        private readonly ILogsService _logsService;

        public OrganizationUnitAppService(IRepository<OrganizationUnit, Guid> repository, OrganizationUnitManager organizationUnitManager,IOrganizationUnitRepository organizationUnitRepository, IIdentityUserRepository userRepository, IIdentityUserAppService userAppService,IIdentityRoleAppService roleAppService, ILogsService logsService)
            : base(repository)
        {
            OrganizationUnitManager = organizationUnitManager;
            OrganizationUnitRepository = organizationUnitRepository;
            UserAppService = userAppService;
            RoleAppService = roleAppService;
            UserRepository = userRepository;           
            _logsService = logsService;
        }
        public async Task<string> GetNextChildCodeAsync()
        {
            //return OrganizationUnitManager.GetNextChildCodeAsync(null);
            System.Text.StringBuilder code = new System.Text.StringBuilder();
            for (; ; )
            {
                code.Clear();
                code.Append(Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10));
                var Query = Repository.Where(s => s.Code == code.ToString());
                var cnt = await AsyncQueryableExecuter.CountAsync(Query);
                if (cnt == 0) break;
            }
            return code.ToString();
        }
        public async Task<string> GetNextChildCodeAsync(Guid parentId)
        {
            //return OrganizationUnitManager.GetNextChildCodeAsync(parentId);
            System.Text.StringBuilder code = new System.Text.StringBuilder();
            for (;;)
            {
                code.Clear();
                code.Append(Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10));
                var Query = Repository.Where(s => s.Code == code.ToString());
                var cnt = await AsyncQueryableExecuter.CountAsync(Query);
                if (cnt == 0) break;
            }
            return code.ToString();
        }
        public async Task<List<OrganizationUnitListDto>> GetAllTreesAsync()
        {
            await CheckGetListPolicyAsync();

           
                var Query = Repository.Where(x => !x.ParentId.HasValue);
                var entities = await AsyncQueryableExecuter.ToListAsync(Query);
                var dtos = await MapToGetListOutputDtosAsync(entities);
                foreach (var dto in dtos)
                {
                    await TraverseTreeAsync(dto, dto.Children);
                }
                return dtos;
           
        }
        public virtual async Task<ListResultDto<OrganizationUnitDto>> GetListByUseridAsync(Guid userId)
        {
            var entitys = await UserRepository.GetOrganizationUnitsAsync(userId);
            return new ListResultDto<OrganizationUnitDto>(
                ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(entitys)
            );
        }
        public virtual async Task<PagedResultDto<IdentityUserDto>> GetUsersAsync(Guid? orgId, GetListIdentityUsersInput userInput)
        {
            if (!orgId.HasValue)
            {
                return await UserAppService.GetListAsync(userInput);
            }
            IEnumerable<IdentityUser> list = new List<IdentityUser>();
            var ou = await OrganizationUnitRepository.GetAsync(orgId.Value);
            var selfAndChildren = await OrganizationUnitRepository.GetAllChildrenWithParentCodeAsync(ou.Code, ou.Id);
            selfAndChildren.Add(ou);
            foreach (var child in selfAndChildren)
            {
                list = Enumerable.Union(list, await OrganizationUnitRepository.GetMembersAsync(
                       child,
                       userInput.Sorting,
                       filter: userInput.Filter
                ));
            }
            //排除admin|guest用户
            //&& p.UserName != "guest"  //guest 用户允许被查询 被选择
            list = list.Where(p => p.UserName != "admin" && p.Id != CurrentUser.Id)
                .WhereIf(!userInput.Filter.IsNullOrWhiteSpace(), p => p.PhoneNumber.Contains(userInput.Filter))
                .WhereIf(!userInput.Name.IsNullOrWhiteSpace(), p => p.Name.Contains(userInput.Name));
            var dtos = new PagedResultDto<IdentityUserDto>(list.Count(),
                ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(list.Skip(userInput.SkipCount).Take(userInput.MaxResultCount).ToList()));
            foreach (var dto in dtos.Items)
            {
                var entitys = await UserRepository.GetOrganizationUnitsAsync(dto.Id);
                dto.ExtraProperties.AddOrUpdate("OrgIds", entitys.Select(x => x.DisplayName));
            }
            return dtos;
        }
        public virtual async Task<PagedResultDto<IdentityUserDto>> GetUsersListAsync(Guid? orgId, GetListIdentityUsersInput userInput)
        {
            IEnumerable<IdentityUser> list = new List<IdentityUser>();
            var ou = await OrganizationUnitRepository.GetAsync(orgId.Value);
            var selfAndChildren = await OrganizationUnitRepository.GetAllChildrenWithParentCodeAsync(ou.Code, ou.Id);
            selfAndChildren.Add(ou);
            foreach (var child in selfAndChildren)
            {
                list = Enumerable.Union(list, await OrganizationUnitRepository.GetMembersAsync(
                       child,
                       userInput.Sorting
                ));
            }
            //排除admin|guest用户
            //&& p.UserName != "guest"  //guest 用户允许被查询 被选择
            list = list.Where(p => p.UserName != "admin")
                .WhereIf(!userInput.Filter.IsNullOrWhiteSpace(), p => p.PhoneNumber.Contains(userInput.Filter))
                .WhereIf(!userInput.Name.IsNullOrWhiteSpace(), p => p.Name.Contains(userInput.Name));
            var dtos = new PagedResultDto<IdentityUserDto>(list.Count(),
                ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(list.Skip(userInput.SkipCount).Take(userInput.MaxResultCount).ToList()));
            foreach (var dto in dtos.Items)
            {
                var entitys = await UserRepository.GetOrganizationUnitsAsync(dto.Id);
                dto.ExtraProperties.AddOrUpdate("OrgIds", entitys.Select(x => x.DisplayName));
            }
            return dtos;
        }
        public virtual async Task<PagedResultDto<IdentityRoleDto>> GetRolesAsync(Guid? orgId, GetIdentityRolesInput roleInput)
        {
            if (!orgId.HasValue)
            {
                return await RoleAppService.GetListAsync(roleInput);
            }
            IEnumerable<IdentityRole> list = new List<IdentityRole>();
            var ou = await OrganizationUnitRepository.GetAsync(orgId.Value);
            var selfAndChildren = await OrganizationUnitRepository.GetAllChildrenWithParentCodeAsync(ou.Code, ou.Id);
            selfAndChildren.Add(ou);
            foreach (var child in selfAndChildren)
            {
                list = Enumerable.Union(list, await OrganizationUnitRepository.GetRolesAsync(
                       child,
                       roleInput.Sorting
                ));
            }
            return new PagedResultDto<IdentityRoleDto>(list.Count(),
                ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list.Skip(roleInput.SkipCount).Take(roleInput.MaxResultCount).ToList())
            );
        }
        public override async Task<PagedResultDto<OrganizationUnitListDto>> GetListAsync(GetListOrganizationUnitInput input)
        {
            await CheckGetListPolicyAsync();
            IQueryable<OrganizationUnit> Query = await base.CreateFilteredQueryAsync(input);

            Query = Query.Where(x => !x.ParentId.HasValue).WhereIf(!string.IsNullOrWhiteSpace(input.Filter),p=>p.DisplayName.Contains(input.Filter));

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);

            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = await MapToGetListOutputDtosAsync(entities);
            foreach (var dto in dtos)
            {
                await TraverseTreeAsync(dto, dto.Children);
            }
            return new PagedResultDto<OrganizationUnitListDto>(totalCount, dtos);

            //return base.GetListAsync(input);
        }
        public virtual async Task<List<OrganizationUnitListDto>> GetChildrenAsync(Guid parentId)
        {
            var list = ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitListDto>>(await OrganizationUnitManager.FindChildrenAsync(parentId));
            await SetLeaf(list);
            return list;
        }

        /// <summary>
        /// 所有父节点集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<OrganizationUnitDto>> GetParentAsync(Guid id)
        {
            var list =new List<OrganizationUnitDto>();
            async Task GetParentAsync(Guid id)
            {
                var entity = await Repository.GetAsync(id);
                if (entity != null)
                {
                    var data = await OrganizationUnitRepository.GetListAsync(p => p.Id == entity.ParentId);
                    if(data!=null && data.Count > 0)
                    {                         
                        list.Add(await MapToGetOutputDtoAsync(data[0]));
                        if(data[0].ParentId!=null)
                            await GetParentAsync(Guid.Parse(data[0].ParentId.ToString()));
                    }                    
                }
            }
            await GetParentAsync(id);
            return list;
        }
        public override async Task<OrganizationUnitDto> CreateAsync(OrganizationUnitCreateDto input)
        {
            await CheckCreatePolicyAsync();          
            var entity = await MapToEntityAsync(input);
            input.MapExtraPropertiesTo(entity);

            //await OrganizationUnitManager.CreateAsync(entity);
            //await CurrentUnitOfWork.SaveChangesAsync();
            await Repository.InsertAsync(entity);

            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "组织机构", Name = "添加", Content = $"添加组织机构，名称[{input.DisplayName}]" });

            return await MapToGetOutputDtoAsync(entity);
        }
        public override async Task<OrganizationUnitDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input)
        {
            await CheckUpdatePolicyAsync();          
            var entity = await this.Repository.GetAsync(id);
            await MapToEntityAsync(input, entity);
            input.MapExtraPropertiesTo(entity);

            await OrganizationUnitManager.UpdateAsync(entity);
            //await CurrentUnitOfWork.SaveChangesAsync();            

            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "组织机构", Name = "更新", Content = $"更新组织机构，名称[{input.DisplayName}]" });

            return await MapToGetOutputDtoAsync(entity);
        }
        public override async Task DeleteAsync(Guid id)
        {
            var entity = await this.Repository.GetAsync(id);
            await base.DeleteAsync(id);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "组织机构", Name = "删除", Content = $"删除组织机构，名称[{entity.DisplayName}]" });
        }
        protected virtual async Task TraverseTreeAsync(OrganizationUnitListDto dto, List<OrganizationUnitListDto> children)
        {
            if (dto.Children.Count == 0)
            {
                children = await GetChildrenAsync(dto.Id);
                dto.Children.AddRange(children);
            }
            if (children == null || !children.Any())
            {
                dto.Children = null;
                await Task.CompletedTask;
                return;
            }
            foreach (var child in children)
            {
                var next = await GetChildrenAsync(child.Id);
                child.Children.AddRange(next);
                await TraverseTreeAsync(child, child.Children);
            }
        }
        /// <summary>
        /// 后面考虑处理存储leaf到数据库,避免这么进行处理
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        protected virtual async Task SetLeaf(List<OrganizationUnitListDto> list)
        {
            foreach (var item in list)
            {
                if ((await OrganizationUnitRepository.GetChildrenAsync(item.Id)).Count == 0)
                {
                    item.SetLeaf();
                }
            }
        }

    }
}
