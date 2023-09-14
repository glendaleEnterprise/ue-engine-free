using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Dtos;
using Glendale.Design.Dtos.ProjectFolder;
using Glendale.Design.Models;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Identity;
using Glendale.Design.Dtos.ProjectFolderUser;
using Glendale.Design.Enums;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 项目目录接口
    /// </summary>
    [Authorize]
    public class ProjectFolderService : CrudAppService<ProjectFolder, ProjectFolderDto, ProjectFolderListDto, Guid, GetListProjectFolderUserInput, ProjectFolderCreateDto, ProjectFolderUpdateDto>, IProjectFolderService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IRepository<Project, Guid> ProjectRepository;
        private readonly IRepository<ProjectFolder, Guid> _repository;
        private readonly IRepository<ProjectFolderUser,Guid> _projectFolderUserRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IdentityUserManager _userManager;
        private readonly ILogsService _logsService;
        private readonly IRepository<Message, Guid> MsgRepository;
        public ProjectFolderService(IRepository<ProjectFolder, Guid> repository, IRepository<Project, Guid> _ProjectRepository, IRepository<ProjectFolderUser, Guid> projectFolderUserRepository, IIdentityUserRepository userRepository, IdentityUserManager userManager, ILogsService logsService, IRepository<Message, Guid> _MsgRepository) : base(repository)
        {
            _repository = repository;
            _projectFolderUserRepository = projectFolderUserRepository;
            _userRepository = userRepository;
            _logsService = logsService;
            _userManager = userManager;
            MsgRepository = _MsgRepository;
            ProjectRepository = _ProjectRepository;
        }

        /// <summary>
        /// 获取目录,包括目录人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<ProjectFolderDto> GetAsync(Guid id)
        {
            var entity = Repository.Include(x => x.ProjectFolderUsers).Where(p => p.Id == id).FirstOrDefault();
            var dto = MapToGetOutputDto(entity);            
            foreach (var item in dto.ProjectFolderUsers)
            {
                var user=await _userRepository.GetAsync(item.UserId);
                item.Name = user.Name;
                item.PhoneNumber = user.PhoneNumber;
                var organizationUnits = await _userManager.GetOrganizationUnitsAsync(user);
                item.OrgNames = organizationUnits.Select(x => x.DisplayName).ToList();                            
            }             
            return dto;            
        }

        /// <summary>
        /// 新增目录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ProjectFolderDto> CreateAsync(ProjectFolderCreateDto input)
        {
            var result= await base.CreateAsync(input);
            Guid id = Guid.NewGuid();
            List<MessageReceive> rlist = new List<MessageReceive>();
            if (input.ProjectFolderUsers != null)
            {            
                foreach (var item in input.ProjectFolderUsers)
                {
                    var entity = ObjectMapper.Map<ProjectFolderUserCreateDto, ProjectFolderUser>(item);
                    entity.ProjectFolderId = result.Id;
                    entity.ProjectId = result.ProjectId;
                    await _projectFolderUserRepository.InsertAsync(entity);

                    rlist.Add(new MessageReceive(Guid.NewGuid())
                    {
                        MessageId = id,
                        ProjectId = input.ProjectId,
                        UserId = item.UserId,
                        IsRead = false,
                        ReadTime = DateTime.Now,
                        ReadCount = 0
                    });
                }
            }
            Message mesMod = new Message(id)
            {
                ProjectId = input.ProjectId,
                Title = $"{CurrentUser.Name} 创建专业策划，您被指定参与专业策划：{input.FolderName}",
                //BodyText = $"<p>批注标题：{entity.Title}</p>",
                BodyText = $"",
                JoinType = MessJoinTypeEnum.专业策划,
                JoinId = result.Id,
                MessageReceives = rlist
            };
            await MsgRepository.InsertAsync(mesMod);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "专业策划", Name = "新增", Content = $"新增专业策划[{input.FolderName}]" });
            return result;
        }

        /// <summary>
        /// 新增目录列表
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> CreateArryAsync(Guid ProjectID, List<ProjectFolderCreateDto> input)
        {
            if(input.Count == 0)
            {
                throw new UserFriendlyException("参数不符合要求");
            }
            //var entitys = ObjectMapper.Map<IEnumerable<ProjectFolderCreateDto>, IEnumerable<ProjectFolder>>(input);

            List<ProjectFolder> folders = new List<ProjectFolder>();
            List<ProjectFolderUser> users = new List<ProjectFolderUser>();
            FolderChildToList(input, null, ProjectID, ref folders, ref users);


            try
            {
                await _repository.InsertManyAsync(folders);
                await _projectFolderUserRepository.InsertManyAsync(users);

                var userids = users.Select(s => s.UserId).Distinct().ToList();

                Guid id = Guid.NewGuid();
                List<MessageReceive> rlist = new List<MessageReceive>();
                foreach (var uid in userids)
                {
                    rlist.Add(new MessageReceive(Guid.NewGuid())
                    {
                        MessageId = id,
                        ProjectId = ProjectID,
                        UserId = uid,
                        IsRead = false,
                        ReadTime = DateTime.Now,
                        ReadCount = 0
                    });
                }
                Message mesMod = new Message(id)
                {
                    ProjectId = ProjectID,
                    Title = $"{CurrentUser.Name} 创建专业策划，您被指定参与其中",
                    //BodyText = $"<p>批注标题：{entity.Title}</p>",
                    BodyText = $"",
                    JoinType = MessJoinTypeEnum.专业策划,
                    MessageReceives = rlist
                };
                await MsgRepository.InsertAsync(mesMod);

                return "True";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void FolderChildToList(List<ProjectFolderCreateDto> list, Guid? ParentId, Guid ProjectID, ref List<ProjectFolder> folders, ref List<ProjectFolderUser> users)
        {
            foreach (var item in list)
            {
                Guid id = Guid.NewGuid();
                folders.Add(new ProjectFolder(id)
                {
                    ProjectId = ProjectID,
                    ParentId = ParentId,
                    FolderName = item.FolderName,
                    Remark = item.Remark
                });
                users.AddRange(item.ProjectFolderUsers.Select(s => new ProjectFolderUser()
                {
                    ProjectId = ProjectID,
                    ProjectFolderId = id,
                    UserId = s.UserId
                }));
                if (item.Children != null && item.Children.Count > 0) FolderChildToList(item.Children, id, ProjectID, ref folders, ref users);
            }
        }

        /// <summary>
        /// 更新目录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ProjectFolderDto> UpdateAsync(Guid id, ProjectFolderUpdateDto input)
        {
            var folderList = await _projectFolderUserRepository.GetListAsync(p => p.ProjectFolderId == id);
            if (folderList != null)
            {
                await _projectFolderUserRepository.DeleteManyAsync(folderList, autoSave: true);
            }
            if (input.ProjectFolderUsers != null)
            {
                foreach (var item in input.ProjectFolderUsers)
                {
                    var entity = ObjectMapper.Map<ProjectFolderUserCreateDto, ProjectFolderUser>(item);
                    entity.ProjectId = input.ProjectId;
                    entity.ProjectFolderId=id;
                    await _projectFolderUserRepository.InsertAsync(entity);
                }
            }            
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "专业策划", Name = "更新", Content = $"更新专业策划[{input.FolderName}]" });
            return await base.UpdateAsync(id, input);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(Guid id)
        {
            var model = await Repository.GetAsync(id);
            var allids = await GetAllChildrenId(model.Id);
            await _projectFolderUserRepository.DeleteAsync(s => allids.Contains(s.Id));
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "专业策划", Name = "删除", Content = $"删除专业策划[{model.FolderName}]" });
            await Repository.DeleteAsync(id);
        }

        [RemoteService(false)]
        public override Task<PagedResultDto<ProjectFolderListDto>> GetListAsync(GetListProjectFolderUserInput input)
        {
            return base.GetListAsync(input);
        }

        /// <summary>
        /// 获取所有目录树(已验证)
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Dtos.TreeDto>> GetTreesAsync(Guid projectId)
        {
            var entitys = await Repository.Where(o => o.ProjectId == projectId).ToListAsync();
            if (entitys.Count > 0)
            {
                //客户确认，目录数据不会太多，希望这里能将用户枚举出来
                var ids = entitys.Select(s => s.Id).ToArray();
                var userlist = await _projectFolderUserRepository.Include(s => s.User).Where(s => ids.Contains(s.ProjectFolderId)).ToListAsync();
                foreach (var item in entitys)
                {
                    item.ProjectFolderUsers = userlist.Where(s => s.ProjectFolderId == item.Id).ToList();
                }
            }
            var res = ObjectMapper.Map<IEnumerable<ProjectFolder>, IEnumerable<Dtos.TreeDto>>(entitys.Where(o => o.ParentId == null));
            
            return res;
        }

        /// <summary>
        /// 我的目录树
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<List<Dtos.TreeDto>> GetMyTreesAsync(Guid projectId)
        {
            var entitys = await Repository.Include(s => s.Project).Where(o => o.ProjectId == projectId).OrderBy(s => s.CreationTime).ToListAsync();
            if (entitys.Count > 0)
            {
                //客户确认，目录数据不会太多，希望这里能将用户枚举出来
                var ids = entitys.Select(s => s.Id).ToArray();
                var userlist = await _projectFolderUserRepository.Include(s => s.User).Where(s => ids.Contains(s.ProjectFolderId)).ToListAsync();
                foreach (var item in entitys)
                {
                    item.ProjectFolderUsers = userlist.Where(s => s.ProjectFolderId == item.Id).ToList();
                }
            }
            var promod = await ProjectRepository.GetAsync(projectId);
            var user = await _userRepository.GetAsync(CurrentUser.Id.Value);
            var userPosition = user.ExtraProperties.GetValueOrDefault("Position")?.ToString();
            //user.UserName == "guest" ||   //现在项目中需设置guest角色，来决定guest在项目中的具体访问权限
            if (userPosition == "1" || (entitys.Count() > 0 && entitys.First().Project.ManageId == CurrentUser.Id))
                return ObjectMapper.Map<List<ProjectFolder>, List<Dtos.TreeDto>>(entitys.Where(o => o.ParentId == null).ToList());

            //1、查找所有包含当前登录人的节点数据
            //2、查找所有父级，移除父级中用户配置包含当前登录人的数据
            //3、再根据节点数据查询所有子级数据

            var result = new List<ProjectFolder>();
            foreach (var item in entitys)
            {
                if (item.ProjectFolderUsers.ToList().Exists(p => p.UserId == CurrentUser.Id))
                    result.Add(item);
            }

            for (int i = 0; i < result.Count; i++)
            {
                if (HandleTreeParentid(entitys, result[i]))
                {
                    result.Remove(result[i]);
                    i--;
                }
            }
            var list = ObjectMapper.Map<List<ProjectFolder>, List<TreeDto>>(result);
            foreach (var item in list)
            {
                item.Children = HandleTreeChildren(entitys, item.Id);
            }
            return list;
        }

        private bool HandleTreeParentid(List<ProjectFolder> prolist, ProjectFolder model)
        {
            bool bl = false;
            for (; ; )
            {
                model = prolist.Where(s => s.Id == model.ParentId).FirstOrDefault();
                if (model == null)
                {
                    break;
                }

                if (model.ProjectFolderUsers.ToList().Exists(p => p.UserId == CurrentUser.Id))
                {
                    bl = true;
                    break;
                }
            }

            return bl;
        }

        private List<TreeDto> HandleTreeChildren(List<ProjectFolder> prolist, Guid? parentid)
        {
            var handlist = prolist.Where(s => s.ParentId == parentid).ToList();

            List<TreeDto> list = new List<TreeDto>();
            foreach (var item in handlist)
            {
                var model = ObjectMapper.Map<ProjectFolder, TreeDto>(item);
                model.Children = HandleTreeChildren(prolist, item.Id);
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获取子节点标识
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<Guid>> GetAllChildrenId(Guid id)
        {
            var entitys = Repository.Where(x => x.ParentId == id).Select(s => s.Id).ToList();
            var list = new List<Guid>();
            foreach (var item in entitys)
            {
                list.Add(item);
                var items = await GetAllChildrenId(item);
                list.AddRange(items);
            }
            return list;
        }

    }
}
