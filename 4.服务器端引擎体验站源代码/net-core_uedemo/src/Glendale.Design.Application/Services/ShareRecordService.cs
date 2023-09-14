
using Glendale.Design.Dtos;
using Glendale.Design.Dtos.Dictionary;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Dtos.ShareRecord;
using Glendale.Design.Enums;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 分享接口
    /// </summary>   
    public class ShareRecordService : CrudAppService<ShareRecord, ShareRecordDto, ShareRecordListDto, Guid, GetListShareRecordInput,
                            ShareRecordCreateUpdateDto, ShareRecordCreateUpdateDto>, IShareRecordAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }        
        private readonly IRepository<Project, Guid> ProjectRepository;
        private readonly ILogsService _logsService;      


        public ShareRecordService(IRepository<ShareRecord, Guid> repository, 
            IRepository<Project,Guid> projectRepository, ILogsService logsService): base(repository)
        {            
            ProjectRepository = projectRepository;
            _logsService = logsService;           
        }

        /// <summary>
        /// 创建分享
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<ShareRecordDto> CreateAsync(ShareRecordCreateUpdateDto input)
        {
            await CheckCreatePolicyAsync();
            var entity = await MapToEntityAsync(input);
            entity.ShareLink = "ShareLink/" + entity.Id;             
            if(entity.IsVerify)
            {
                entity.Auth = new Random().Next(1000,10000).ToString();
            }            
            SetIdForGuids(entity);
            entity = await Repository.InsertAsync(entity);                       

            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override Task<ShareRecordDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }

        [RemoteService(false)]
        public override Task<ShareRecordDto> UpdateAsync(Guid id, ShareRecordCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
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
        /// 分享查询【带分页】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<ShareRecordListDto>> GetListAsync(GetListShareRecordInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Where(o => o.CreatorId == CurrentUser.Id)
                .WhereIf(!input.Keyword.IsNullOrEmpty(), t => t.ShareTile.Contains(input.Keyword))
                .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            foreach (var entity in entitys)
            {                 
                entity.ShareState = IsExceed(entity);
            }
            var list = ObjectMapper.Map<IEnumerable<ShareRecord>, IReadOnlyList<ShareRecordListDto>>(entitys);
           
            return new PagedResultDto<ShareRecordListDto>(totalCount, list);
        }



        /// <summary>
        /// 累加分享数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Addpvm(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            if (entity != null)
            {
                entity.PvmCount += 1;
                await Repository.UpdateAsync(entity);
            }
        }


        /// <summary>
        /// 是否需要授权码
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAuth(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            if (!entity.IsNullOrEmpty())
            {
                var shareState = IsExceed(entity);
                if (shareState == StareStateEnum.BeyondShare) 
                { 
                    throw new UserFriendlyException("已经超出分享次数"); 
                }
                if (shareState == StareStateEnum.Overdue) 
                { 
                    throw new UserFriendlyException("已经超过分享期限了"); 
                }
                return entity.IsVerify;
            }
            throw new UserFriendlyException("分享链接不存在");
        }


        /// <summary>
        /// 校验授权码是否正确
        /// </summary>
        /// <param name="id">记录Id</param>
        /// <param name="auth">授权码</param>
        /// <returns></returns>
        [AllowAnonymous]         
        public async Task<ShareRecordDto> AuthVerify(Guid id, string auth)
        {
            var entity =await Repository.GetAsync(id);
            if (!entity.IsNullOrEmpty())
            {
                var shareState = IsExceed(entity);
                if (shareState == StareStateEnum.BeyondShare)
                {
                    throw new UserFriendlyException("已经超出分享次数");
                }
                if (shareState == StareStateEnum.Overdue)
                {
                    throw new UserFriendlyException("已经超过分享期限了");
                }
                if ((entity.Auth == auth) || (!entity.IsVerify))
                {
                    return  MapToGetOutputDto(entity); 
                }
                throw new UserFriendlyException("授权码无效");
            }
            throw new UserFriendlyException("分享链接不存在");
        }
        
        /// <summary>
        /// 分享状态判断
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private StareStateEnum IsExceed(ShareRecord entity)
        {             
            if (entity.EffectiveDay > 0 && (DateTime.Now - entity.CreationTime).Days > entity.EffectiveDay)
            {
                return StareStateEnum.Overdue;
            }
            if (entity.ShareCount > 0 && entity.PvmCount > entity.ShareCount)
            {
                return StareStateEnum.BeyondShare;
            }
            return StareStateEnum.Open;
        }
    }
}
