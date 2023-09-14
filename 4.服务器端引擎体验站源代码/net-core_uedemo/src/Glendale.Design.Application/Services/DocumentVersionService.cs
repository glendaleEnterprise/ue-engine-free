using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glendale.Design.Dtos;
using Glendale.Design.Dtos.Document;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Enums;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    [Authorize]
    public class DocumentVersionService : CrudAppService<DocumentVersion, DocumentVersionDto, DocumentVersionListDto, Guid, GetListDocumentVersionInput, DocumentVersionCreateUpdateDto, DocumentVersionCreateUpdateDto>, IDocumentVersionService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IRepository<Document, Guid> _docRepository;
        private readonly IRepository<DocumentLog, Guid> _docLogRepository;         
        private readonly ILogsService _logsService;

        public DocumentVersionService(IRepository<DocumentVersion, Guid> repository, IRepository<DocumentLog, Guid> docLogRepository, 
            IRepository<Document, Guid> docRepository,ILogsService logsService) : base(repository)
        {
            _docLogRepository = docLogRepository;
            _docRepository = docRepository;
            _logsService = logsService;
        }

        /// <summary>
        /// 获取全部模型名称
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(true)]
        public async Task<List<string>> GetModelNamesAsync()
        {
            return await Repository.Where(x => x.Status==DocStatusEnum.Succeed && (x.DocumentType.Equals(DocTypeEnum.Model) || x.DocumentType.Equals(DocTypeEnum.CAD))
            && x.ModelName != null).Select(x => x.ModelName).ToListAsync();
        }

        /// <summary>
        /// 上传新版本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<DocumentVersionDto> CreateAsync(DocumentVersionCreateUpdateDto input)
        {
            var documentEntity = await _docRepository.GetAsync(input.DocumentId);
            var maxVersionNo= Repository.Where(p => p.DocumentId == input.DocumentId).Select(s => s.VersionNo).Max();
            if (input.VersionNo == 0)
            {
                input.VersionNo = maxVersionNo + 0.1;
            }else if (input.VersionNo < 0)
            {
                input.VersionNo = (int)maxVersionNo+1;
            }
            var entity= ObjectMapper.Map<DocumentVersionCreateUpdateDto, DocumentVersion>(input);
            entity.DocumentType = documentEntity.DocumentType;
            entity.ModelFormat = documentEntity.ModelFormat;
            entity = await Repository.InsertAsync(entity);
            await _docLogRepository.InsertAsync(new DocumentLog(entity.DocumentId, string.Format("上传新版本V{0}", entity.VersionNo.ToString("0.0"))));
            if (input.IsCurrent)
            {
                var oldEntity = await Repository.FirstOrDefaultAsync(o => o.IsCurrent && o.DocumentId == entity.DocumentId);
                if (oldEntity != null)
                {
                    oldEntity.IsCurrent = false;
                    await Repository.UpdateAsync(oldEntity, true);                     
                }
                
                if (documentEntity != null)
                {
                    documentEntity.ModelName=input.ModelName;
                    documentEntity.Status = DocStatusEnum.Await;
                    documentEntity.Size = input.Size;
                    documentEntity.Exception = String.Empty;
                    await _docRepository.UpdateAsync(documentEntity, true);
                }
            }
            await _docLogRepository.InsertAsync(new DocumentLog(input.DocumentId, "上传新版本"));
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "模型版本", Name = "新增", Content = $"上传新版本[{documentEntity.DocumentName}]" });
            return MapToGetOutputDto(entity);
        }
        /// <summary>
        /// 查询模型版本【带分页】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<DocumentVersionListDto>> GetListAsync(GetListDocumentVersionInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Where(o=>o.DocumentId==input.DocumentId)  
                .WhereIf(!string.IsNullOrEmpty(input.Keyword),o=>o.ModelName.Contains(input.Keyword));
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<DocumentVersion>, IReadOnlyList<DocumentVersionListDto>>(entitys);
            return new PagedResultDto<DocumentVersionListDto>(totalCount, list);
        }
        /// <summary>
        /// 设置版本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SetCurrent(Guid id)
        {
            //获取新版本            
            var newEntity = await Repository.GetAsync(id);
            newEntity.IsCurrent = true; 
            
            //获取模型文件信息
            var docEntity = await _docRepository.GetAsync(newEntity.DocumentId);
            docEntity.ModelName = newEntity.ModelName;
            docEntity.Size = newEntity.Size;
            docEntity.VersionNo = newEntity.VersionNo;
            docEntity.Status = newEntity.Status;
            docEntity.DocumentVersion.Add(newEntity);
            //当前版本信息
            var oldEntity = await Repository.FirstOrDefaultAsync(o => o.IsCurrent && o.DocumentId == newEntity.DocumentId);
            await _docLogRepository.InsertAsync(new DocumentLog(newEntity.DocumentId, string.Format("切换至版本V{0}", newEntity.VersionNo.ToString("0.0"))));
            if (oldEntity != null)
            {
                oldEntity.IsCurrent = false;
                docEntity.DocumentVersion.Add(oldEntity);
            }
            //await _docLogRepository.InsertAsync(new DocumentLog(id, $"切换至版本V{newEntity.VersionNo.ToString("0.0")}"));
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "模型版本", Name = "更新", Content = $"切换至版本V{newEntity.VersionNo.ToString("0.0")}" });
            await _docRepository.UpdateAsync(docEntity); 
        }

        /// <summary>
        /// 更新版本轻量化状态
        /// </summary>
        /// <returns></returns>
        //public async Task UpdateDocumentVerStateAsync(Guid id, DocumentVersionCreateUpdateDto input)
        //{
        //    var entity = await Repository.GetAsync(id);
        //    await MapToEntityAsync(input, entity);
        //    if (entity.IsCurrent)
        //    {
        //        await Repository.UpdateAsync(entity, autoSave: false);
        //        //var logEntity = new DocumentLog { DocumentId = entity.DocumentId, Remark = string.Format("更新[{0}]版本状态：{1}", entity.VersionNo, entity.Status.GetDescription()) };
        //       // await _docLogRepository.InsertAsync(logEntity, autoSave: true);
        //    }
        //    else
        //    {
        //        var docDto = ObjectMapper.Map<DocumentVersionCreateUpdateDto, DocumentCreateUpdateDto>(input);
        //    }
        //}
        //public async Task UpdateSync(Guid id, SyncStatusEnum syncStatus)
        //{
        //    var entity = await Repository.GetAsync(id);
        //    entity.SyncStatus = syncStatus;
        //    await Repository.UpdateAsync(entity, autoSave: false);
        //    var logEntity = new DocumentLog { DocId = entity.DocId, Remark = string.Format("同步[{0}]版本数据", entity.DocVer) };
        //    await DocLogRepository.InsertAsync(logEntity, autoSave: true);
        //}
        /// <summary>
        /// 同步轻量化名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task SyncModelName(Guid id, string modelName)
        {
            var entity = await this.GetEntityByIdAsync(id);
            entity.ModelName = modelName;
            entity.Status = DocStatusEnum.Await;
            if (entity.IsCurrent)
            {
                var Document = await _docRepository.FirstOrDefaultAsync(x => x.Id.Equals(entity.DocumentId));
                if (Document != null)
                {
                    entity.Document.ModelName = modelName;
                    entity.Document.Status = DocStatusEnum.Await;
                   // entity.Document.DocumentLog = new List<DocumentLog> { new DocumentLog (entity.DocumentId, "文件已上传" ) };                    
                }
            }
            await Repository.UpdateAsync(entity);
        }


        /// <summary>
        /// 同步轻量化状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="statusDescription"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task SyncModelStatus(Guid id, DocStatusEnum status,string statusDescription)
        {
            var entity = await this.GetEntityByIdAsync(id);
            //状态值无变化，日志则无需记录入库
            if (entity.Status != status)
            {
                await _docLogRepository.InsertAsync(new DocumentLog (entity.DocumentId, $"更新状态[{status.GetDescription()}]  " + statusDescription));
            }

            entity.Status = status;
            entity.Exception = statusDescription;
            if (entity.IsCurrent)
            {
                var Document = await _docRepository.FirstOrDefaultAsync(x => x.Id.Equals(entity.DocumentId));
                if (Document != null)
                {
                    entity.Document = Document;
                    entity.Document.Status = status;
                    entity.Document.Exception = statusDescription;
                }
            }            
            await Repository.UpdateAsync(entity);
        }
        /// <summary>
        /// 获取待轻量化BIM文件(获取三十天内的数据)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task<List<DocumentVersionDto>> GetLightweightModelAsync()
        {
            var entitys = await Repository.Where(x => (x.Status.Equals(DocStatusEnum.Await) || x.Status.Equals(DocStatusEnum.Running))
            && (x.DocumentType.Equals(DocTypeEnum.Model) || x.DocumentType.Equals(DocTypeEnum.GIS))
            && (x.CreationTime >= DateTime.Now.AddDays(-30) || x.LastModificationTime >= DateTime.Now.AddDays(-30))).ToListAsync();
            return ObjectMapper.Map<List<DocumentVersion>, List<DocumentVersionDto>>(entitys);
        }
        /// <summary>
        /// 获取待轻量化CAD文件(获取三十天内的数据)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task<List<DocumentVersionDto>> GetLightweightCADAsync()
        {
            var entitys = await Repository.Where(x => (x.Status.Equals(DocStatusEnum.Await) || x.Status.Equals(DocStatusEnum.Running))         
            && x.DocumentType.Equals(DocTypeEnum.CAD) 
            && (x.CreationTime >= DateTime.Now.AddDays(-30) || x.LastModificationTime >= DateTime.Now.AddDays(-30))).ToListAsync();
            return ObjectMapper.Map<List<DocumentVersion>, List<DocumentVersionDto>>(entitys);
        }
        //protected override async Task<IQueryable<DocumentVersion>> CreateFilteredQueryAsync(GetListDocumentVersionInput input)
        //{
        //    var Query = await base.CreateFilteredQueryAsync(input);
        //    Query = Query.Where(o => o.DocumentId == input.DocumentId) 
        //        .WhereIf(!input.Keyword.IsNullOrEmpty(), o => o.ModelName.Contains(input.Keyword));
        //    return Query;
        //}
    }
}
