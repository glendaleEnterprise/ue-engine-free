using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Dtos;
using Glendale.Design.Dtos.Project;
using Glendale.Design.Dtos.ProjectFolderUser;
using Glendale.Design.Dtos.ProjectImage;
using Glendale.Design.Dtos.ProjectUser;
using Glendale.Design.Enums;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Linq;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 项目
    /// </summary>
    [Authorize]
    public class ProjectService :CrudAppService<Project,ProjectDto,ProjectListDto,Guid,GetListProjectInput,ProjectCreateDto,ProjectUpdateDto>,IProjectService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }       
        private readonly IRepository<ProjectUser, Guid> _projectUserRepository;
        private readonly IRepository<ProjectImage, Guid> _projectImageRepository;
        private readonly IRepository<RoleOrgJoin, Guid> _roleOrgJoinRepository;
        private readonly IRepository<ProjectFolderUser, Guid> _projectFolderUserRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IdentityUserManager _userManager;
        private readonly ILogsService _logsService;
        private readonly IPermissionGrantRepository PermissionGrantRepository;
        public ProjectService(IRepository<Project, Guid> repository, IdentityUserManager userManager,
            IRepository<ProjectUser,Guid> projectUserRepository, ILogsService logsService, IIdentityUserRepository userRepository,
            IRepository<RoleOrgJoin, Guid> roleOrgJoinRepository,
            IRepository<ProjectFolderUser, Guid> projectFolderUserRepository,
            IRepository<ProjectImage, Guid> projectImageRepository,
            IPermissionGrantRepository _PermissionGrantRepository) : base(repository)
        {
            _userManager = userManager;
            _projectUserRepository = projectUserRepository;
            _logsService = logsService;
            _projectImageRepository = projectImageRepository;
            _userRepository = userRepository;
            _roleOrgJoinRepository = roleOrgJoinRepository;
            _projectFolderUserRepository = projectFolderUserRepository;
            PermissionGrantRepository = _PermissionGrantRepository;
        }


        /// <summary>
        /// 新建项目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ProjectDto> CreateAsync(ProjectCreateDto input)
        {
            input.ManageId = CurrentUser.Id;
            var dto = await base.CreateAsync(input);
            if (input.ProjectImages != null)
            {
                foreach (var item in input.ProjectImages)
                {
                    await _projectImageRepository.InsertAsync(new ProjectImage()
                    {
                        BlobName = item.BlobName,
                        IsCover = item.IsCover,
                        ProjectId = dto.Id,
                    });
                }
            }
            if (input.ProjectUsers != null)
            {
                foreach (var item in input.ProjectUsers)
                {
                    await _projectUserRepository.InsertAsync(new ProjectUser()
                    {
                        IsManager = item.IsManager,
                        UserId = item.UserId,
                        ProjectId = dto.Id
                    });
                }
            }
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "项目管理", Name = "新增", Content = $"新增[{input.ProjectName}]" });
            return dto;
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>      
        public override async Task<ProjectDto> UpdateAsync(Guid id, ProjectUpdateDto input)
        {
            //移除
            var imageList = await _projectImageRepository.GetListAsync(p => p.ProjectId == id);
            if (imageList != null)
            {
                await _projectImageRepository.DeleteManyAsync(imageList, autoSave: true);
            }
            var userList = await _projectUserRepository.GetListAsync(p => p.ProjectId == id);
            if (userList != null)
            {
                await _projectUserRepository.DeleteManyAsync(userList, autoSave: true);
            }

            if (input.ProjectImages != null)
            {
                foreach (var item in input.ProjectImages)
                {
                    item.ProjectId = id;
                    await _projectImageRepository.InsertAsync(ObjectMapper.Map<ProjectImageUpdateDto, ProjectImage>(item));
                }
            }
            if (input.ProjectUsers != null)
            {
                foreach (var item in input.ProjectUsers)
                {
                    item.ProjectId = id;
                    await _projectUserRepository.InsertAsync(ObjectMapper.Map<ProjectUserUpdateDto, ProjectUser>(item));
                }
            }
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "项目管理", Name = "更新", Content = $"更新[{input.ProjectName}]" });
            return await base.UpdateAsync(id, input);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "项目管理", Name = "删除", Content = $"删除[{entity.ProjectName}]" });
            await base.DeleteAsync(id);
        }

        /// <summary>
        /// 获取对象（已验证）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<ProjectDto> GetAsync(Guid id)
        {
            var entity = await Repository.Include(x => x.ProjectImages).Include(x => x.ProjectUsers).Include(x => x.Creator).Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            var dto = await MapToGetOutputDtoAsync(entity);
            if (dto != null)
            {
                foreach (var proUser in dto.ProjectUsers)
                {
                    var user = await _userManager.GetByIdAsync(proUser.UserId);
                    proUser.Name = user.Name;
                    proUser.UserName = user.UserName;
                    proUser.PhoneNumber = user.PhoneNumber;
                    proUser.Position = user.ExtraProperties.GetValueOrDefault("Position")?.ToString();
                    var organizationUnits = await _userManager.GetOrganizationUnitsAsync(user);
                    proUser.OrgNames = organizationUnits.Select(x => x.DisplayName).ToList();                     
                }
            }
            return dto;
        }

        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<ProjectListDto>> GetListAsync(GetListProjectInput input)
        {
            var roles = await _userRepository.GetRolesAsync(CurrentUser.Id.Value);
            var rids = roles.Select(s => s.Id).ToList();
            var orgids = await _roleOrgJoinRepository.Where(s => rids.Contains(s.RoleId)).Select(s => s.OrgId).ToListAsync();
            var user = await _userRepository.GetAsync(CurrentUser.Id.Value);
            var pusers = await _projectFolderUserRepository.Where(s => s.UserId == CurrentUser.Id).Select(s => s.ProjectId).Distinct().ToListAsync();

            var position = user.ExtraProperties.GetValueOrDefault("Position")?.ToString();
            var Query = Repository.Include(x => x.ProjectImages).Include(x => x.ManageInfo)
                .Where(s => s.ManageId == CurrentUser.Id //项目负责人
                    || (CurrentUser.UserName == "admin") //admin
                    || (position == "1" && s.OrgId.HasValue && orgids.Contains(s.OrgId.Value)) //领导关联
                    || pusers.Contains(s.Id) //专业策划关联人员
                    || s.IsPublic == true) //公开项目
                .WhereIf(input.IsPublic != null, o => o.IsPublic == input.IsPublic) //是否公开
                .WhereIf(input.Keyword.IsNotNullOrEmpty(), o => o.ProjectName.Contains(input.Keyword) || o.ShortName.Contains(input.Keyword))//关键字搜索
                .WhereIf(input.Tag.IsNotNullOrEmpty(), o => o.Tags.Contains(input.Tag))
                .WhereIf(input.Year > 0, o => input.Year == o.BeginDate.Year);

            var totalCount =await  AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await  AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<Project>, List<ProjectListDto>>(entitys);
            foreach (var item in dtos)
            {
                item.CoverImage = item.ProjectImages.ToList().Any() ? item.ProjectImages.ToList()[0].BlobName : string.Empty;
            }
            return new PagedResultDto<ProjectListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 我参与的
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectListDto>> GetListMineJoinAsync(GetListProjectInput input)
        {
            var pusers = await _projectFolderUserRepository.Where(s => s.UserId == CurrentUser.Id).Select(s => s.ProjectId).Distinct().ToListAsync();

            var Query = Repository.Include(x => x.ProjectImages).Include(x => x.ManageInfo)
                .Where(s => pusers.Contains(s.Id) //专业策划关联人员
                    || (CurrentUser.UserName == "admin"))
                .WhereIf(input.Keyword.IsNotNullOrEmpty(), o => o.ProjectName.Contains(input.Keyword) || o.ShortName.Contains(input.Keyword))//关键字搜索
                .WhereIf(input.Tag.IsNotNullOrEmpty(), o => o.Tags.Contains(input.Tag))
                .WhereIf(input.Year > 0, o => input.Year == o.BeginDate.Year);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<Project>, List<ProjectListDto>>(entitys);
            foreach (var item in dtos)
            {
                item.CoverImage = item.ProjectImages.ToList().Any() ? item.ProjectImages.ToList()[0].BlobName : string.Empty;
            }
            return new PagedResultDto<ProjectListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 我创建的
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectListDto>> GetListMineAllAsync(GetListProjectInput input)
        {
            var pusers = await _projectFolderUserRepository.Where(s => s.UserId == CurrentUser.Id).Select(s => s.ProjectId).Distinct().ToListAsync();

            var Query = Repository.Include(x => x.ProjectImages).Include(x => x.ManageInfo)
                .Where(s => s.ManageId == CurrentUser.Id) //项目负责人
                .WhereIf(input.Keyword.IsNotNullOrEmpty(), o => o.ProjectName.Contains(input.Keyword) || o.ShortName.Contains(input.Keyword))//关键字搜索
                .WhereIf(input.Tag.IsNotNullOrEmpty(), o => o.Tags.Contains(input.Tag))
                .WhereIf(input.Year > 0, o => input.Year == o.BeginDate.Year);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<Project>, List<ProjectListDto>>(entitys);
            foreach (var item in dtos)
            {
                item.CoverImage = item.ProjectImages.ToList().Any() ? item.ProjectImages.ToList()[0].BlobName : string.Empty;
            }
            return new PagedResultDto<ProjectListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 领导管理的项目
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectListDto>> GetListMineManageAsync(GetListProjectInput input)
        {
            var roles = await _userRepository.GetRolesAsync(CurrentUser.Id.Value);
            var rids = roles.Select(s => s.Id).ToList();
            var orgids = await _roleOrgJoinRepository.Where(s => rids.Contains(s.RoleId)).Select(s => s.OrgId).ToListAsync();
            var user = await _userRepository.GetAsync(CurrentUser.Id.Value);

            var position = user.ExtraProperties.GetValueOrDefault("Position")?.ToString();
            var Query = Repository.Include(x => x.ProjectImages).Include(x => x.ManageInfo)
                .Where(s => (position == "1" && s.OrgId.HasValue && orgids.Contains(s.OrgId.Value))) //领导关联
                .WhereIf(input.Keyword.IsNotNullOrEmpty(), o => o.ProjectName.Contains(input.Keyword) || o.ShortName.Contains(input.Keyword))//关键字搜索
                .WhereIf(input.Tag.IsNotNullOrEmpty(), o => o.Tags.Contains(input.Tag))
                .WhereIf(input.Year > 0, o => input.Year == o.BeginDate.Year);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<Project>, List<ProjectListDto>>(entitys);
            foreach (var item in dtos)
            {
                item.CoverImage = item.ProjectImages.ToList().Any() ? item.ProjectImages.ToList()[0].BlobName : string.Empty;
            }
            return new PagedResultDto<ProjectListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 查询项目人员（已弃用）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ProjectUserListDto>> GetProjectUsers(GetListProjectUserInput input)
        {
            var total = 0;
            var query = from u in _projectUserRepository.Where(p => p.ProjectId == input.ProjectId)
                        orderby u.CreationTime descending
                        select new ProjectUserListDto
                        {
                            IsManager = u.IsManager,
                            UserId = u.UserId,
                            Id = u.Id,
                            ProjectId = u.ProjectId,
                        };

            total = query.DistinctBy(p => new { p.Id }).Count();
            var list = query.DistinctBy(p => new { p.Id }).Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            if (list != null)
            {
                foreach (var proUser in list)
                {
                    var user = await _userManager.GetByIdAsync(proUser.UserId);
                    proUser.Name = user.Name;
                    proUser.UserName = user.UserName;
                    proUser.PhoneNumber = user.PhoneNumber;
                    proUser.Position = user.ExtraProperties.GetValueOrDefault("Position")?.ToString();
                    var organizationUnits = await _userManager.GetOrganizationUnitsAsync(user);
                    proUser.OrgNames = organizationUnits.Select(x => x.DisplayName).ToList();
                }
            }
            return new PagedResultDto<ProjectUserListDto>(total, list);
        }

        /// <summary>
        /// 查询登录用户的项目权限
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<List<PermissionGrantProjectUserDto>> GetProjectUserPermissionAsync(Guid ProjectId)
        {
            var rNameList = new List<string>();
            if (CurrentUser.UserName == "admin")
            {
                rNameList.Add("admin");
            }
            if(CurrentUser.UserName == "guest")
            {
                rNameList.AddRange(CurrentUser.Roles);
            }

            var promod = await Repository.GetAsync(ProjectId);
            if (promod.IsPublic && CurrentUser.UserName != "guest")
            {
                var guest = await _userManager.FindByNameAsync("guest");
                var roles = await _userManager.GetRolesAsync(guest);
                rNameList.AddRange(roles);
            }

            var pfList = await _projectFolderUserRepository.Where(s => s.ProjectId == ProjectId && s.UserId == CurrentUser.Id)
                .Select(s => s.UserRoleNames).GroupBy(s => s).Select(s => s.Key).ToListAsync();
            if (rNameList.Count == 0 && (pfList == null || pfList.Count == 0))
            {
                throw new Volo.Abp.UserFriendlyException("请联系相关人员配置项目角色");
            }
            foreach (var item in pfList)
            {
                if (!string.IsNullOrEmpty(item))
                    rNameList.AddRange(item.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries));
            }
            if (rNameList.Count == 0 && (rNameList == null || rNameList.Count == 0))
            {
                throw new Volo.Abp.UserFriendlyException("请联系相关人员配置项目角色");
            }

            var permissionList = await PermissionGrantRepository.GetListAsync(p => p.Name.Contains("Design.Business") && rNameList.Contains(p.ProviderKey));
            permissionList = permissionList.Where(s => s.Name.Where(s => s == '.').Count() > 2).ToList();
            if (permissionList.Count() == 0) return null;

            var list = new List<PermissionGrantProjectUserDto>();
            foreach (var item in permissionList.Select(s => "Design.Business." + s.Name.Split('.')[2]).ToArray().Distinct())
            {
                list.Add(new PermissionGrantProjectUserDto()
                {
                    //TenantId = item.TenantId,
                    Name = item,
                    Children = permissionList.Where(s => (s.Name != item && s.Name.IndexOf(item) >= 0) ||
                    //模型列表下的创建视点和创建标签，没有按照前面遵守的命名规则来，这里改了后动全身，还是挺麻烦的
                    (item.IndexOf("Documents") >= 0 && (s.Name.IndexOf("Viewpoint.Add") >= 0 || s.Name.IndexOf("Label.Add") >= 0)))
                    .Select(s => new PermissionGrantProjectUserDto()
                    {
                        //TenantId = s.TenantId,
                        Name = s.Name,
                        ProviderName = s.ProviderName,
                        ProviderKey = s.ProviderKey,
                    })
                    .ToList()
                });
            }

            return list;
        }

        /// <summary>
        /// 判断用户是否负责项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> isJoin(Guid id)
        {
            var data = await Repository.Where(s => s.ManageId == id).ToListAsync();
            if (data.Count() == 0)
            {
                return true;
            }
            return false;
        }
    }
}
