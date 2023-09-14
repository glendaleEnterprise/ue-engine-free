using Glendale.Design.Dtos;
using Glendale.Design.Dtos.Combine;
using Glendale.Design.Dtos.Document;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 合摸接口
    /// </summary>
    public class CombineService : CrudAppService<Combine, CombineDto, CombineListDto, Guid, GetListCombineInput,CombineCreateUpdateDto, CombineCreateUpdateDto>, ICombineAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IRepository<CombineDetail, Guid> CombineDetailRepository;
        private readonly IRepository<CombineFlatten, Guid> CombineFlattenRepository;
        private readonly IRepository<Document, Guid> DocumentRepository;
        private readonly IDocumentService _documentService;
        private readonly ILogsService _logsService;

        private readonly IRepository<CombineLog, Guid> CombineLogRepository;
        public CombineService(IRepository<Combine, Guid> repository, IRepository<CombineDetail, Guid> detailRepository, IRepository<CombineFlatten, Guid> _CombineFlattenRepository, IRepository<Document, Guid> documentRepository,IRepository<CombineLog, Guid> combineLogRepository, ILogsService logsService, IDocumentService documentService) : base(repository)
        {
            CombineDetailRepository = detailRepository;
            CombineFlattenRepository = _CombineFlattenRepository;
            DocumentRepository = documentRepository;
          
            CombineLogRepository = combineLogRepository;
            _documentService = documentService;
            _logsService = logsService;
        }

        /// <summary>
        /// 创建合模表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<CombineDto> CreateAsync(CombineCreateUpdateDto input)
        {
            await CheckCreatePolicyAsync();
            var entity = await MapToEntityAsync(input);
            SetIdForGuids(entity);
            var CombineDetails = new List<CombineDetail>();
            if(input.CombineDetails!=null && input.CombineDetails.Any())
            {            
                input.CombineDetails.ToList().ForEach(x =>
                {
                    CombineDetails.Add(new CombineDetail(GuidGenerator.Create(), entity.Id, x.DocumentId, x.Matrix,x.MatrixGis, x.ModelConfig));
                });
                entity.CombineDetails = null;
            }
            var CombineFlattens = new List<CombineFlatten>();
            if(input.CombineFlattens!=null && input.CombineFlattens.Any())
            {
                input.CombineFlattens.ToList().ForEach(x =>
                {
                    CombineFlattens.Add(new CombineFlatten(GuidGenerator.Create(), entity.Id, x.FlattenType, x.FlattenName, x.FlattenHeight, x.FlattenScope));
                });
                entity.CombineFlattens = null;
            }
            if (input.CombineCuts != null && input.CombineCuts.Any())
            {
                input.CombineCuts.ToList().ForEach(x =>
                {
                    CombineFlattens.Add(new CombineFlatten(GuidGenerator.Create(), entity.Id, x.FlattenType, x.FlattenName, x.FlattenHeight, x.FlattenScope));
                });
                entity.CombineCuts = null;
            }
            var CombineLogs = new List<CombineLog>();
            CombineLogs.Add(new CombineLog { Remark = "创建合模", CombineId = entity.Id });

            entity = await Repository.InsertAsync(entity, autoSave: true);
            if (CombineLogs.Count() > 0) await CombineLogRepository.InsertManyAsync(CombineLogs);
            if (CombineDetails.Count() > 0) await CombineDetailRepository.InsertManyAsync(CombineDetails);
            if (CombineFlattens.Count() > 0) await CombineFlattenRepository.InsertManyAsync(CombineFlattens);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "合模列表", Name = "新增", Content = $"新增合模[{input.CombineName}]" });
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async override Task<CombineDto> UpdateAsync(Guid id, CombineCreateUpdateDto input)
        {
            var entity = await Repository.GetAsync(id);
            await MapToEntityAsync(input, entity);
            if(input.CombineDetails != null && input.CombineDetails.Count > 0)
            {
                //合模绑定的模型不允许调整
                entity.CombineDetails = null;
            }
            if (input.CombineFlattens != null && input.CombineFlattens.Count > 0)
            {
                input.CombineFlattens = null;
            }
            if (input.CombineCuts != null && input.CombineCuts.Count > 0)
            {
                input.CombineCuts = null;
            }

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新预览图
        /// </summary>
        /// <param name="id">合模数据标识</param>
        /// <param name="BlobName">预览图编码</param>
        /// <returns></returns>
        public async Task<CombineDto> UpdateBlobNameAsync(Guid id, string BlobName)
        {
            var entity = await Repository.GetAsync(id);
            entity.BlobName = BlobName;

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新模型主视角
        /// </summary>
        /// <param name="id">合模数据标识</param>
        /// <param name="Camera">模型主视角</param>
        /// <returns></returns>
        public async Task<CombineDto> UpdateCameraAsync(Guid id, string Camera)
        {
            var entity = await Repository.GetAsync(id);
            entity.Camera = Camera;

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新合模信息
        /// </summary>
        /// <param name="id">数据标识</param>
        /// <param name="SceneConfig">模型场景配置修改</param>
        /// <returns></returns>
        public async Task<CombineDto> UpdateSceneConfigAsync(Guid id, string SceneConfig)
        {
            var entity = await Repository.GetAsync(id);
            entity.SceneConfig = SceneConfig;

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新模型信息
        /// </summary>
        /// <param name="id">数据标识</param>
        /// <param name="ModelConfig">模型配置修改</param>
        /// <returns></returns>
        public async Task<ShowCombineDetailDto> UpdateModelConfigAsync(Guid id, Guid DocumentId, string ModelConfig)
        {
            var entity = await CombineDetailRepository.Where(s => s.CombineId == id && s.DocumentId == DocumentId).FirstOrDefaultAsync();
            entity.ModelConfig = ModelConfig;

            entity = await CombineDetailRepository.UpdateAsync(entity);
            return ObjectMapper.Map<CombineDetail, ShowCombineDetailDto>(entity);
        }

        /// <summary>
        /// 更新模型位置信息
        /// </summary>
        /// <param name="id">数据标识</param>
        /// <param name="Matrix">模型位置修改</param>
        /// <returns></returns>
        public async Task<ShowCombineDetailDto> UpdateMatrixAsync(Guid id, Guid DocumentId, string Matrix)
        {
            var entity = await CombineDetailRepository.Where(s => s.CombineId == id && s.DocumentId == DocumentId).FirstOrDefaultAsync();
            entity.Matrix = Matrix;

            entity = await CombineDetailRepository.UpdateAsync(entity);
            return ObjectMapper.Map<CombineDetail, ShowCombineDetailDto>(entity);
        }

        /// <summary>
        /// 合模查询【带分页】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<CombineListDto>> GetListAsync(GetListCombineInput input)
        {
            //var listFolderId = new List<Guid>();
            //var folders = await _documentService.GetChildTree(input.FolderId, Enums.DocTypeEnum.Folder);
            //if (folders != null)
            //{
            //    listFolderId = folders.Select(p => p.Id).ToList();
            //}
            //listFolderId.Add(input.FolderId);


            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Include(x => x.CombineDetails).ThenInclude(x => x.Document).Include(x => x.Creator)
                .WhereIf(!input.CombineName.IsNullOrEmpty(), o => o.CombineName.StartsWith(input.CombineName))
                .WhereIf(input.ProjectId != null, o => o.ProjectId == input.ProjectId)
                .Where(p => p.FolderId == input.FolderId);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<Combine>, IReadOnlyList<CombineListDto>>(entitys);
            //foreach (var item in list)
            //{
            //    item.CombineCuts = item.CombineFlattens.Where(s => s.FlattenType == 1).ToList();
            //    item.CombineFlattens = item.CombineFlattens.Where(s => s.FlattenType == 0).ToList();
            //}
            return new PagedResultDto<CombineListDto>(totalCount, list);
        }

        /// <summary>
        /// 用来查询合模的模型信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/app/combine/GetDocument")]
        public async Task<List<DocumentListDto>> GetDocument(Guid id)
        {
            var ids = await Repository.Include(x => x.CombineDetails).Include(x=>x.CombineFlattens).Where(x => x.Id.Equals(id)).SelectMany(x => x.CombineDetails.Select(x => x.DocumentId)).ToListAsync();
            var entitys = await DocumentRepository.Where(x => ids.Contains(x.Id)).ToListAsync();
            return ObjectMapper.Map<IEnumerable<Document>, List<DocumentListDto>>(entitys);
        }


        /// <summary>
        /// 合模查询【不带分页】
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="combineName"></param>
        /// <returns></returns>
        //[HttpGet("/api/app/combine/GetAllListAsync")]
        public  async Task<List<CombineListDto>> GetAllListAsync(Guid? projectId,string combineName)
        {
            var Query = await base.CreateFilteredQueryAsync(new GetListCombineInput());
            Query = Query.Include(x => x.CombineDetails).ThenInclude(x => x.Document).Include(x=>x.CombineFlattens)
                .WhereIf(!combineName.IsNullOrEmpty(), o => o.CombineName == combineName)
                .WhereIf(projectId != null, o => o.ProjectId == projectId);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            return ObjectMapper.Map<IEnumerable<Combine>, List<CombineListDto>>(entitys);
        }

        /// <summary>
        /// 获取合模对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<CombineDto> GetAsync(Guid id)
        {
            var entity = await base.Repository.Include(x => x.CombineFlattens).Include(o => o.CombineDetails).ThenInclude(x => x.Document).FirstOrDefaultAsync(o => o.Id == id);

            //entity.CombineCuts = entity.CombineFlattens.Where(s => s.FlattenType == 1).ToList();
            //entity.CombineFlattens = entity.CombineFlattens.Where(s => s.FlattenType == 0).ToList();
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 查询模型已经被合过的 合模集合
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<CombineListDto>> HasCombine(Guid docId)
        {
            var ids=CombineDetailRepository.Where(x=>x.DocumentId==docId).Select(x=>x.CombineId);           
            var entitys = await Repository.Where(x => ids.Contains(x.Id)).ToListAsync();
            return ObjectMapper.Map<IEnumerable<Combine>, List<CombineListDto>>(entitys);
        }

        /// <summary>
        /// 设置公开/私有
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="isOpen"></param>
        /// <returns></returns>
        public async Task<List<CombineDto>> SetOpen(Guid[] ids, bool isOpen)
        {
            var result=new List<CombineDto>();
            if (ids.Length==0)
                throw new UserFriendlyException("ids参数不能为空");
            for(int i = 0; i < ids.Length; i++)
            {
                await CombineLogRepository.InsertAsync(new CombineLog() { CombineId = ids[i], Remark = isOpen ? "设置公开" : "设置私有" });
                var id = ids[i];
                var entity = await Repository.GetAsync(id);
                entity.IsOpen = isOpen;
                var data = await Repository.UpdateAsync(entity);
                result.Add(MapToGetOutputDto(data));
            }
            var docNames = string.Join(", ", result.Select(s => s.CombineName).ToArray());
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "合模列表", Name = (isOpen ? "公开" : "私有"), Content = $"将合模[{docNames}]{(isOpen ? "公开" : "私有")}" });
            return result;
        }
    }
}
