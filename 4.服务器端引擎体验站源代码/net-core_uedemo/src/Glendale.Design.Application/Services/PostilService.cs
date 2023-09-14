using Glendale.Design.Dtos.Postil;
using Glendale.Design.Enums;
using Glendale.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 批注
    /// </summary>
    public class PostilService : CrudAppService<Postil, PostilDto, PostilListDto, Guid, GetListPostilInput,PostilCreateUpdateDto, PostilCreateUpdateDto>, IPostilAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }        
        private readonly IRepository<Document, Guid> _documentRepository;
        private readonly IRepository<DocumentVersion, Guid> _documentVerRepository;
        private readonly IRepository<Combine,Guid> _combineRepository;
        private readonly IRepository<Message, Guid> MsgRepository;
        public PostilService(IRepository<Postil, Guid> repository
            , IRepository<Document, Guid> documentRepository, IRepository<DocumentVersion, Guid> documentVerRepository, IRepository<Combine, Guid> combineRepository, IRepository<Message, Guid> _MsgRepository)
            : base(repository)
        {
           
            _documentRepository = documentRepository;
            _documentVerRepository = documentVerRepository;
            _combineRepository = combineRepository;
            MsgRepository = _MsgRepository;
        }

        /// <summary>
        /// 获取批注对象详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<PostilDto> GetAsync(Guid id)
        {
            var data= await Repository.Include(x=>x.Creator).Where(x => x.Id == id).FirstAsync();
            return  MapToGetOutputDto(data);          
        }

        /// <summary>
        /// 新增批注
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PostilDto> CreateAsync(PostilCreateUpdateDto input)
        {
            await CheckCreatePolicyAsync();
            var entity = await MapToEntityAsync(input);
            entity.Status = PostilStatusEnum.待处理;

            if (input.SceneType==SceneTypeEnum.Multi)
            {
               var combineEntity= await _combineRepository.GetAsync(input.DocumentId);
                entity.HandlerUserId = input.HandlerUserId;
                entity.DocumentName = combineEntity.CombineName;
            }
            else
            {
                #region 获取docVerId
                var docVerList = await _documentVerRepository.GetListAsync(p => p.DocumentId == input.DocumentId && p.IsCurrent == true);
                if (docVerList != null && docVerList.Count > 0)
                {
                    entity.DocumentVersionId = docVerList[0].Id;
                }
                #endregion
                var documentEntity = await _documentRepository.GetAsync(input.DocumentId);
                entity.HandlerUserId = input.HandlerUserId;
                entity.DocumentName = documentEntity.DocumentName;
                entity.DocumentVersionNo = documentEntity.VersionNo;
            }             
            SetIdForGuids(entity);
             
            entity = await Repository.InsertAsync(entity);
            Guid id = Guid.NewGuid();
            List<MessageReceive> rlist = new List<MessageReceive>()
            {
                new MessageReceive(Guid.NewGuid()){
                    MessageId = id,
                    ProjectId = input.ProjectId,
                    UserId = input.HandlerUserId.Value,
                    IsRead = false,
                    ReadTime = DateTime.Now,
                    ReadCount = 0
                }
            };
            Message mesMod = new Message(id)
            {
                ProjectId = entity.ProjectId,
                Title = "您有一条待处理的批注信息",
                //BodyText = $"<p>批注标题：{entity.Title}</p>",
                BodyText = $"",
                JoinType = MessJoinTypeEnum.批注,
                JoinId = entity.Id,
                MessageReceives = rlist
            };
            await MsgRepository.InsertAsync(mesMod);
            return await MapToGetOutputDtoAsync(entity);
        } 

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<PostilListDto>> GetListAsync(GetListPostilInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Include(x => x.Creator)
                .WhereIf(!input.Title.IsNullOrEmpty(), o => o.Title.StartsWith(input.Title))
                .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId)
                .WhereIf(input.DocumentId != null, o => o.DocumentId == input.DocumentId)
                .WhereIf(input.Category!=null ,o=>o.PostilCategory==input.Category)
                .WhereIf(input.Status!=null, o => o.Status==input.Status)
                .WhereIf(input.CreatorId!=null,o=>o.CreatorId==input.CreatorId)                
                .WhereIf(input.HandlerUserId!=null,o=>o.HandlerUserId==input.HandlerUserId);
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<Postil>, IReadOnlyList<PostilListDto>>(entitys);            
            return new PagedResultDto<PostilListDto>(totalCount, list);
        }

        /// <summary>
        /// 分页（发起的）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<PostilListDto>> GetLaunchAsync(GetListPostilInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Include(x => x.Creator)
                .WhereIf(!input.Title.IsNullOrEmpty(), o => o.Title.StartsWith(input.Title))
                .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId)
                .WhereIf(input.DocumentId != null, o => o.DocumentId == input.DocumentId)
                .WhereIf(input.Category != null, o => o.PostilCategory == input.Category)
                .WhereIf(input.Status != null, o => o.Status == input.Status)
                .WhereIf(true, o => o.CreatorId == CurrentUser.Id);
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<Postil>, IReadOnlyList<PostilListDto>>(entitys);             
            return new PagedResultDto<PostilListDto>(totalCount, list);
        }
        /// <summary>
        /// 分页(处理)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<PostilListDto>> GetHandleAsync(GetListPostilInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Include(x => x.Creator)
                .WhereIf(!input.Title.IsNullOrEmpty(), o => o.Title.StartsWith(input.Title))
                .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId)
                .WhereIf(input.DocumentId != null, o => o.DocumentId == input.DocumentId)
                .WhereIf(input.Category != null, o => o.PostilCategory == input.Category)
                .WhereIf(input.Status != null, o => o.Status == input.Status)
                .WhereIf(true, o => o.HandlerUserId == input.HandlerUserId);
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<Postil>, IReadOnlyList<PostilListDto>>(entitys);            
            return new PagedResultDto<PostilListDto>(totalCount, list);
        }

        /// <summary>
        /// 删除批注
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        /// <summary>
        /// 修改状态(已处理/已关闭)
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="status">状态</param>
        /// <param name="handlerRemark">说明</param>
        /// <returns></returns>
        public async Task<PostilDto> UpdateStatus(Guid id, PostilStatusEnum status,string handlerRemark)
        {
            var entity=await Repository.GetAsync(id);
            if(status== PostilStatusEnum.待处理)
            {
                throw new UserFriendlyException("只能修改为已处理或已关闭");
            }else if(status == PostilStatusEnum.已处理)
            {
                entity.HandlerDateTime = DateTime.Now;
                entity.HandlerRemark = handlerRemark;
            }else if (status == PostilStatusEnum.已关闭)
            {
                entity.CloseDateTime = DateTime.Now;
            }
            entity.Status = status; 
            var result=await Repository.UpdateAsync(entity, true);

            Guid mid = Guid.NewGuid();
            List<MessageReceive> rlist = new List<MessageReceive>()
            {
                new MessageReceive(Guid.NewGuid()){
                    MessageId = mid,
                    ProjectId = entity.ProjectId,
                    UserId = entity.CreatorId.Value,
                    IsRead = false,
                    ReadTime = DateTime.Now,
                    ReadCount = 0
                }
            };
            Message mesMod = new Message(mid)
            {
                ProjectId = entity.ProjectId,
                Title = "您有一条待处理的批注信息",
                //BodyText = $"<p>批注标题：{entity.Title}</p>",
                BodyText = $"",
                JoinType = MessJoinTypeEnum.批注,
                JoinId = entity.Id,
                MessageReceives = rlist
            };
            await MsgRepository.InsertAsync(mesMod);
            return MapToGetOutputDto(result);
        }

    }
}
