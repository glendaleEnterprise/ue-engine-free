using System;
using System.Linq;
using System.Threading.Tasks;
using Glendale.Design.Dtos.Document;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Models;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Glendale.Design.Enums;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.ObjectMapping;
using System.IO;
using Volo.Abp.Identity;
using Volo.Abp;
using Glendale.Design.Dtos;

namespace Glendale.Design.Services
{
    [Authorize]
    public class DocumentService : CrudAppService<Document, DocumentDto, DocumentListDto, Guid, GetListDocumentInput, DocumentCreateUpdateDto, DocumentCreateUpdateDto>, IDocumentService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IRepository<DocumentVersion, Guid> _documentVersionRepository;
        private readonly IRepository<DocumentLog, Guid> _docLogRepository;
        private readonly IIdentityUserRepository UserRepository;
        private readonly IRepository<Label, Guid> _bimLabelRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IdentityUserManager UserManager;
        private readonly ILogsService _logsService;
        private readonly IRepository<Postil, Guid> _postilRepository;
        private readonly IRepository<CombineDetail, Guid> _combineDetailRepository;
        private readonly IRepository<Combine, Guid> _combineRepository;

        public DocumentService(IRepository<Document, Guid> repository, IRepository<DocumentLog, Guid> docLogRepository, IIdentityUserRepository userRepository, IRepository<Label, Guid> bimLabelRepository, IRepository<Project, Guid> projectRepository, ILogsService logsService, IdentityUserManager userManager, IRepository<Postil, Guid> postilRepository, IRepository<CombineDetail, Guid> combineDetailRepository, IRepository<Combine, Guid> combineRepository, IRepository<DocumentVersion, Guid> documentVersionRepository) : base(repository)
        {
            _docLogRepository = docLogRepository;
            UserRepository = userRepository;
            _bimLabelRepository = bimLabelRepository;
            _projectRepository = projectRepository;
            _logsService = logsService;
            UserManager = userManager;
            _postilRepository = postilRepository;
            _combineDetailRepository = combineDetailRepository;
            _combineRepository = combineRepository;
            _documentVersionRepository = documentVersionRepository;
        }

        /// <summary>
        /// 上传新模型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>        
        public override async Task<DocumentDto> CreateAsync(DocumentCreateUpdateDto input)
        {
            var result = await base.CreateAsync(input);
            var documentVer = ObjectMapper.Map<DocumentCreateUpdateDto, DocumentVersion>(input);
            documentVer.DocumentId = result.Id;
            documentVer.IsCurrent = true;
            var docVer = await _documentVersionRepository.InsertAsync(documentVer);
            result.DocumentVersion = ObjectMapper.Map<DocumentVersion, DocumentVersionDto>(docVer);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "模型列表", Name = "新增", Content = $"上传了一个新模型[{input.DocumentName}]" });
            await _docLogRepository.InsertAsync(new DocumentLog(result.Id, "文件已上传"));
            return result;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<DocumentDto> UpdateAsync(Guid id, DocumentCreateUpdateDto input)
        {
            return await base.UpdateAsync(id, input);
        }

        /// <summary>
        /// 更新预览图
        /// </summary>
        /// <param name="id">合模数据标识</param>
        /// <param name="BlobName">预览图编码</param>
        /// <returns></returns>
        public async Task<DocumentDto> UpdateBlobNameAsync(Guid id, string BlobName)
        {
            var entity = await Repository.GetAsync(id);
            entity.BlobName = BlobName;

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新模型信息
        /// </summary>
        /// <param name="id">合模数据标识</param>
        /// <param name="Camera">模型主视角</param>
        /// <returns></returns>
        public async Task<DocumentDto> UpdateCameraAsync(Guid id, string Camera)
        {
            var entity = await Repository.GetAsync(id);
            entity.Camera = Camera;

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新模型信息
        /// </summary>
        /// <param name="id">数据标识</param>
        /// <param name="SceneConfig">模型场景配置修改</param>
        /// <returns></returns>
        public async Task<DocumentDto> UpdateSceneConfigAsync(Guid id, string SceneConfig)
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
        public async Task<DocumentDto> UpdateModelConfigAsync(Guid id, string ModelConfig)
        {
            var entity = await Repository.GetAsync(id);
            entity.ModelConfig = ModelConfig;

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新模型位置信息
        /// </summary>
        /// <param name="id">数据标识</param>
        /// <param name="Matrix">模型位置修改</param>
        /// <returns></returns>
        public async Task<DocumentDto> UpdateMatrixAsync(Guid id, string Matrix)
        {
            var entity = await Repository.GetAsync(id);
            entity.Matrix = Matrix;

            entity = await Repository.UpdateAsync(entity);
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 分页查询文件列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async override Task<PagedResultDto<DocumentListDto>> GetListAsync(GetListDocumentInput input)
        {
            var Query = Repository.Include(x => x.Creator).Include(x => x.DocumentVersion)
                .WhereIf(input.DocumentType != null, o => input.DocumentType.Contains(o.DocumentType))
                .WhereIf(true, o => o.ProjectFolderId == input.ProjectFolderId)
                .WhereIf(input.Keyword.IsNotNullOrEmpty(), o => o.DocumentName.Contains(input.Keyword));
            //.WhereIf(CurrentUser.UserName.Equals("guest"), o => o.IsPublic == true);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<Document>, List<DocumentListDto>>(entitys);
            return new PagedResultDto<DocumentListDto>(totalCount, dtos);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(Guid id)
        {
            var model = await base.GetAsync(id);
            string nameArr = model.DocumentName;
            if (model.DocumentType == DocTypeEnum.CAD || model.DocumentType == DocTypeEnum.Model)
            {
                //手动删除对应的合模记录
                var combineList = _combineDetailRepository.Where(p => p.DocumentId == id).DistinctBy(p => p.CombineId).ToList();
                if (combineList != null)
                {
                    foreach (var d in combineList)
                    {
                        await _combineRepository.DeleteAsync(d.CombineId);
                    }
                }
                await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "模型列表", Name = "删除", Content = $"将模型[{nameArr}]删除" });
            }
            await base.DeleteAsync(id);
        }

        /// <summary>
        /// 删除多个
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task Mdelete(Guid[] ids)
        {
            if (ids.Length == 0)
                return;
            for (int i = 0; i < ids.Length; i++)
            {
                await DeleteAsync(ids[i]);
            }
        }

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task<DocumentDto> GetAsync(Guid id)
        {
            return await base.GetAsync(id);
        }

        /// <summary>
        /// 查询多个
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<List<DocumentListDto>> GetArrayAsync(Guid[] ids)
        {
            var Query = Repository.Where(p => ids.Contains(p.Id));
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<Document>, List<DocumentListDto>>(entitys);
            return dtos;
        }

        /// <summary>
        /// 移动文件目录
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="documentIds"></param>
        /// <returns></returns>
        public async Task UpdateMoveAsync(Guid folderId, [FromBody] Guid[] documentIds)
        {
            var entitys = await Repository.Where(x => documentIds.Contains(x.Id)).ToListAsync();
            foreach (var entity in entitys)
            {
                entity.ProjectFolderId = folderId;
                await _docLogRepository.InsertAsync(new DocumentLog(entity.Id, "移动文件"));
            }
            //var docNames = string.Join(", ", entitys.Select(s => s.DocumentName).ToArray());
            var docNames = $"将模型[{entitys.Select(s => s.DocumentName).First()}]等模型移动";
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "模型列表", Name = "移动", Content = $"将模型[{docNames}]移动" });
            await Repository.UpdateManyAsync(entitys, true);
        }

        /// <summary>
        /// 公开文件
        /// </summary>
        /// <param name="isPublic"></param>
        /// <param name="documentIds"></param>
        /// <returns></returns>
        public async Task UpdatePublicAsync(bool isPublic, [FromBody] Guid[] documentIds)
        {
            var entitys = await Repository.Where(x => documentIds.Contains(x.Id)).ToListAsync();
            foreach (var entity in entitys)
            {
                entity.IsPublic = isPublic;
                await _docLogRepository.InsertAsync(new DocumentLog(entity.Id, isPublic ? "设置公开" : "设置私有"));
            }
            var docNames = string.Join(", ", entitys.Select(s => s.DocumentName).ToArray());
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "模型列表", Name = (isPublic ? "公开" : "私有"), Content = $"将模型[{docNames}]{(isPublic ? "公开" : "私有")}" });
            await Repository.UpdateManyAsync(entitys, true);
        }

        /// <summary>
        /// word转Html
        /// </summary>
        /// <param name="blobName">带扩展名，如：602ace13-7336-4a48-b1f9-d7f7d7259572.docx</param>
        /// <param name="page"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<dynamic> GetWordHtmlAsync(string blobName, int? page)
        {
            string folder = blobName.Split(".")[0];
            long fileSize = 0;
            var entity = Repository.Where(p => p.ModelName == folder).FirstOrDefault();
            if (entity != null)
            {
                fileSize = entity.Size;
            }
            if (fileSize == 0)
            {
                var entityAttch = _bimLabelRepository.Where(p => p.BlobName == folder).FirstOrDefault();
                //if (entityAttch != null)
                //{
                //    fileSize = entityAttch.ByteSize;
                //}
            }
            var total = 0;
            string rootPath = AppContext.BaseDirectory; // 根路径 
            string result = "";

            if (fileSize / 1024 / 1024 >= 20)
            {
                string dirPath = Path.Combine(rootPath, "wwwroot", "uploads", folder, "page");//路径                
                var filePath = Path.Combine(dirPath, String.Format("{0}.docx", (page ?? 0)));
                result = ce.office.extension.WordHelper.ToHtml(filePath);

                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                var files = directoryInfo.GetFiles("*.docx");
                total = files.Length;
            }
            else
            {
                string dirPath = Path.Combine(rootPath, "wwwroot", "uploads");//路径
                var filePath = Path.Combine(dirPath, blobName);
                result = ce.office.extension.WordHelper.ToHtml(filePath);
            }
            return new
            {
                result,
                total
            };
        }

        /// <summary>
        /// Excel转Html
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public Task<string> GetExcelHtmlAsync(string fileName)
        {
            string rootPath = AppContext.BaseDirectory; // 根路径
            string filePath = Path.Combine(rootPath, "wwwroot", "uploads", fileName);//路径 
            return Task.FromResult(ce.office.extension.ExcelHelper.ToHtml(filePath));
        }

        /// <summary>
        /// 查询每种图模格式的数量以及上传人的统计
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<Dictionary<int, GraphModuleStatistics>> GetNumOfEachFormat(Guid id)
        {
            var dic = new Dictionary<int, GraphModuleStatistics>();
            var entity = await Repository.GetListAsync(t => t.ProjectId == id);
            var graphList = entity.Where(t => t.DocumentType == DocTypeEnum.CAD || t.DocumentType == DocTypeEnum.Model || t.DocumentType == DocTypeEnum.GIS);

            var allUser = await UserRepository.GetListAsync();

            if (graphList.Count() > 0)
            {
                var groupByFileType = from t in graphList
                                      group t by t.ModelFormat into t2
                                      select new GroupByFileTypeDto
                                      {
                                          name = t2.Key,
                                          value = t2.Count().ToString()
                                      };

                var groupByUser = from t in graphList
                                  group t by t.CreatorId into t2
                                  select new GroupByUserDto
                                  {
                                      UserName = allUser.FirstOrDefault(t => t.Id == t2.Key).Name,
                                      UploadNum = graphList.Count(t => t.CreatorId == t2.Key),
                                  };

                dic.Add((int)DocTypeEnum.Model, new GraphModuleStatistics { GroupByFileTypeDto = groupByFileType.ToList(), GroupByUserDto = groupByUser.ToList(), TotalNum = graphList.Count() });
            }
            return dic;
        }

        #region 目录人员权限


        /// <summary>
        /// 获取目录已设置人员，子目录未设置自动继承上一级的
        /// </summary>         
        /// <param name="id">目录Id</param>
        /// <returns></returns>
        //public async Task<List<DocumentUserListDto>> GetUsersAsync(Guid id)
        //{
        //    var list = new List<DocumentUserListDto>();
        //    var document = await Repository.GetAsync(id);
        //    if (document != null)
        //    {
        //        var userIds = DocumentUserRepository
        //            .Where(p => p.DocumentId == document.Id )
        //            .Select(o => new DocumentUserListDto { UserId = o.UserId,  PermissionsDto =o.Permissions.FromJson<DocumentUserPermissionsDto>() }).ToList();
        //        list.AddRange(userIds);
        //        if (userIds.Count == 0 && document.ParentId != null)
        //        {
        //            async Task GetParentAsync(Guid parentId)
        //            {
        //                var doc = await Repository.GetAsync(parentId);
        //                if (doc != null)
        //                {
        //                    var userIds2 = DocumentUserRepository
        //                        .Where(p => p.DocumentId == doc.Id )
        //                        .Select(o => new DocumentUserListDto { UserId = o.UserId,PermissionsDto=o.Permissions.FromJson<DocumentUserPermissionsDto>() }).ToList();
        //                    list.AddRange(userIds2);
        //                    if (userIds2.Count == 0 && doc.ParentId != null)
        //                    {
        //                        await GetParentAsync(Guid.Parse(doc.ParentId.ToString()));
        //                    }
        //                }
        //            }
        //            await GetParentAsync(Guid.Parse(document.ParentId.ToString()));
        //        }
        //    }            
        //    foreach (var item in list)
        //    {
        //        var userEntity =await UserManager.GetByIdAsync(item.UserId);
        //        if (userEntity != null)
        //        {
        //            item.Name = userEntity.Name;
        //            item.PhoneNumber = userEntity.PhoneNumber;                     
        //            var organizationUnits = await UserManager.GetOrganizationUnitsAsync(userEntity);
        //            item.OrgNames = organizationUnits.Select(x => x.DisplayName).ToList();
        //        }
        //    } 
        //    return list;
        //}


        /// <summary>
        /// 获取当前用户在该目录权限
        /// 如果当前目录未设置权限，从上一级目录继承（以此类推，直到根目录）
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <param name="folderId">目录Id</param>
        /// <returns></returns>
        //public async Task<DocumentUserPermissionsDto> GetFolderPermission(Guid projectId, string folderId)
        //{
        //    DocumentUserPermissionsDto permission = new DocumentUserPermissionsDto();             

        //    var projectEntity = await ProjectRepository.GetAsync(projectId);
        //    if (projectEntity != null)
        //    {
        //        //当前用户是项目创建者，拥有所有权限
        //        if (projectEntity.CreatorId == CurrentUser.Id)
        //        {
        //            permission.IsDelete = true;
        //            permission.IsDownload = true;
        //            permission.IsFileLink = true;
        //            permission.IsMove = true;
        //            permission.IsPostil = true;
        //            permission.IsPublic = true;
        //            permission.IsShare = true;
        //            permission.IsUpload = true;
        //            permission.IsViewpoint = true;
        //            permission.IsSwitchVersion = true;
        //            permission.IsVersionThan = true;
        //            permission.IsBrowse = true;
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(folderId))
        //            {
        //                #region 递归获取目录权限                         
        //                async Task GetParentAsync(Guid parentId)
        //                {
        //                    var document = await Repository.Where(x => x.Id.Equals(parentId)).FirstOrDefaultAsync();
        //                    if (document != null)
        //                    {
        //                        var folderUserEntity =await DocumentUserRepository.Where(p => p.DocumentId == document.Id && p.UserId==CurrentUser.Id)
        //                            .Select(o => new DocumentUserDto { UserId = o.UserId, Permissions=o.Permissions }).FirstOrDefaultAsync();
        //                        if (folderUserEntity != null )
        //                        {
        //                            folderUserEntity.PermissionsDto = folderUserEntity.Permissions.FromJson<DocumentUserPermissionsDto>();
        //                            permission.IsDelete = folderUserEntity.PermissionsDto.IsDelete;
        //                            permission.IsDownload = folderUserEntity.PermissionsDto.IsDownload;
        //                            permission.IsFileLink = folderUserEntity.PermissionsDto.IsFileLink;
        //                            permission.IsMove = folderUserEntity.PermissionsDto.IsMove;
        //                            permission.IsPostil = folderUserEntity.PermissionsDto.IsPostil;
        //                            permission.IsPublic = folderUserEntity.PermissionsDto.IsPublic;
        //                            permission.IsShare = folderUserEntity.PermissionsDto.IsShare;
        //                            permission.IsUpload = folderUserEntity.PermissionsDto.IsUpload;
        //                            permission.IsViewpoint = folderUserEntity.PermissionsDto.IsViewpoint;
        //                            permission.IsSwitchVersion = folderUserEntity.PermissionsDto.IsSwitchVersion;
        //                            permission.IsVersionThan = folderUserEntity.PermissionsDto.IsVersionThan;
        //                            permission.IsBrowse = folderUserEntity.PermissionsDto.IsBrowse;
        //                        }
        //                        else
        //                        {
        //                            if (document.ParentId != null)
        //                            {
        //                                await GetParentAsync(Guid.Parse(document.ParentId.ToString()));
        //                            }
        //                        }
        //                    }
        //                }
        //                await GetParentAsync(Guid.Parse(folderId));
        //                #endregion             
        //            }                    
        //        }
        //    }            
        //    return permission;
        //}
        #endregion

        [AllowAnonymous]
        [HttpPost("/api/app/document/PakCallBack")]
        public async Task PakCallBack( Paramentrs data)
        {
            Newtonsoft.Json.Linq.JObject configObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(data.data);
            string LightweightName = string.Empty;
            string Msg;
            int status = 0;
            if (configObj["LightweightName"] != null)
                LightweightName = configObj["LightweightName"].ToString();
            if (configObj["Msg"] != null)
                Msg = configObj["Msg"].ToString();
            if (configObj["status"] != null)
                status = int.Parse(configObj["status"].ToString());
            Document? doc = Repository.Where(x => x.ModelName == LightweightName).FirstOrDefault();
            if (status == 1)
            {
                if (doc != null)
                {
                    doc.Status = DocStatusEnum.Succeed;
                    await Repository.UpdateAsync(doc, true);
                }
            }
            else
            {
                if (doc != null)
                {
                    doc.Status = DocStatusEnum.Failure;
                    await Repository.UpdateAsync(doc, true);
                }
            }
        }
    }
    public class Paramentrs
    {
        public string data { get; set; }
    }
}