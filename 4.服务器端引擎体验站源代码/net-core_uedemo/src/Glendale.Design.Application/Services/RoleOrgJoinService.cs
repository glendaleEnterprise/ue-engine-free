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
using Glendale.Design.Dtos.RoleOrgJoin;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 项目目录接口
    /// </summary>
    [Authorize]
    public class RoleOrgJoinService : CrudAppService<RoleOrgJoin, RoleOrgJoinDto, RoleOrgJoinListDto, Guid, GetListRoleOrgJoinInput, RoleOrgJoinCreateUpdateDto, RoleOrgJoinCreateUpdateDto>, IRoleOrgJoinAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IRepository<RoleOrgJoin, Guid> Repository;
        private readonly IIdentityUserRepository UserRepository;
        private readonly IdentityUserManager UserManager;
        private readonly ILogsService LogsService;
        public RoleOrgJoinService(IRepository<RoleOrgJoin, Guid> _Repository, IIdentityUserRepository _UserRepository, IdentityUserManager _UserManager, ILogsService _LogsService) : base(_Repository)
        {
            Repository = _Repository;
            UserRepository = _UserRepository;
            UserManager = _UserManager;
            LogsService = _LogsService;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<RoleOrgJoinDto> GetAsync(Guid id)
        {
            var entity = Repository.Where(p => p.Id == id).FirstOrDefault();
            return MapToGetOutputDto(entity);   
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<RoleOrgJoinDto> CreateAsync(RoleOrgJoinCreateUpdateDto input)
        {
            return await base.CreateAsync(input);
        }

        /// <summary>
        /// 新增列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<string> CreateArryAsync(List<RoleOrgJoinCreateUpdateDto> input)
        {
            try
            {
                if(input.Count == 0 || input.GroupBy(s => s.RoleId).Select(s => s.Key).Count() > 1)
                {
                    throw new Volo.Abp.UserFriendlyException("参数不符合要求");
                }

                var id = input.First().RoleId;
                await Repository.DeleteAsync(s => s.RoleId == id, autoSave: true);

                var entitys = ObjectMapper.Map<IEnumerable<RoleOrgJoinCreateUpdateDto>, IEnumerable<RoleOrgJoin>>(input);

                await Repository.InsertManyAsync(entitys);

                return "True";
            }
            catch (Exception)
            {
                return "False";
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<RoleOrgJoinDto> UpdateAsync(Guid id, RoleOrgJoinCreateUpdateDto input)
        {        
            return await base.UpdateAsync(id, input);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async override Task<PagedResultDto<RoleOrgJoinListDto>> GetListAsync(GetListRoleOrgJoinInput input)
        {
            var Query = Repository.WhereIf(input.RoleId.HasValue, o => input.RoleId == o.RoleId)
                .WhereIf(input.OrgId.HasValue, o => o.OrgId == input.OrgId);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<RoleOrgJoin>, List<RoleOrgJoinListDto>>(entitys);
            return new PagedResultDto<RoleOrgJoinListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<RoleOrgJoinListDto>> GetAllListAsync(GetListRoleOrgJoinInput input)
        {
            var list = await Repository.WhereIf(input.RoleId.HasValue, o => input.RoleId == o.RoleId)
                .WhereIf(input.OrgId.HasValue, o => o.OrgId == input.OrgId).ToListAsync();

            return ObjectMapper.Map<List<RoleOrgJoin>, List<RoleOrgJoinListDto>>(list);
        }
    }
}
