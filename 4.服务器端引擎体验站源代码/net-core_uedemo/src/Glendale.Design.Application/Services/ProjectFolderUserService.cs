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

namespace Glendale.Design.Services
{
    /// <summary>
    /// 项目目录接口
    /// </summary>
    [Authorize]
    public class ProjectFolderUserService : CrudAppService<ProjectFolderUser, ProjectFolderUserDto, ProjectFolderUserListDto, Guid, GetListProjectFolderUserInput, ProjectFolderUserCreateDto, ProjectFolderUserCreateDto>, IProjectFolderUserService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IRepository<ProjectFolderUser, Guid> _repository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly ILogsService _logsService;
        public ProjectFolderUserService(IRepository<ProjectFolderUser, Guid> repository, IRepository<IdentityUser, Guid> userRepository, ILogsService logsService) : base(repository)
        {
            _repository = repository;
            _logsService = logsService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 获取项目用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<ProjectFolderUserListDto>> GetProjectUserListAsync(GetListProjectFolderUserInput input)
        {
            //查询项目中所有用户
            var proUserList = await Repository.Include(s => s.User).Include(s => s.ProjectFolder).Where(o => input.ProjectId == o.ProjectId).ToListAsync();

            //获取项目中所有用户的标识信息
            var uids = proUserList.Select(s => s.UserId).Distinct().ToArray();


            List<ProjectFolderUserListDto> list = new List<ProjectFolderUserListDto>();
            //获取项目中的用户信息
            foreach (var uid in uids)
            {
                var proUsers = proUserList.Where(s => s.UserId == uid).ToList();
                var mod = new ProjectFolderUserListDto()
                {
                    UserId = uid,
                    Name = proUsers.First().User.Name,
                    PhoneNumber = proUsers.First().User.PhoneNumber,
                    ProRoles = new List<string>(),
                    ProFolders = new List<string>()
                };
                foreach (var prouser in proUsers)
                {
                    if(!string.IsNullOrEmpty(prouser.UserRoleNames))
                        mod.ProRoles.AddRange(prouser.UserRoleNames.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries));
                    mod.ProFolders.Add(prouser.ProjectFolder.FolderName);
                }
                list.Add(mod);
            }
            return list;
        }
    }
}
