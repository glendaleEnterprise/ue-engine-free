using Glendale.Design.Dtos.Document;
using Glendale.Design.Dtos.File;
using Glendale.Design.Enums;
using Glendale.Design.Jobs;
using Glendale.Design.Core;
using Glendale.Design.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using System.Net.Http;
using Volo.Abp;
using Glendale.Design.Dtos.DocumentVer;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Data;
using Glendale.Design.Models;
using File = System.IO.File;
using Spire.Doc;
using Glendale.Design.Dtos;
using Volo.Abp.Domain.Repositories;
using Document = Glendale.Design.Models.Document;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Glendale.Design.Services
{
    /// <summary>
    /// 文档文件上传接口
    /// </summary>
    [Authorize]
    public class DocumentHandleAppService : DesignAppService
    {
        private readonly IDocumentService DocumentService;
        private readonly IDocumentVersionService DocumentVerService;
        private readonly IBackgroundJobManager BackgroundJobManager;       
        private readonly ModelFileOption ModelOption;
       
        private readonly AbpDbConnectionOptions DbConnectionOptions;
        private readonly IHttpClientFactory HttpClient;
        private readonly HttpHelp Client;
        private readonly ILogsService _logsService;
        private readonly IRepository<Document, Guid> DocumentRepository;
        private readonly IRepository<DocumentVersion, Guid> DocumentVerRepository;
        private readonly IRepository<DocumentLog, Guid> DocLogRepository;
        private readonly ILogger<DocumentHandleAppService> _logger;
        
        public DocumentHandleAppService(IDocumentService documentService,IDocumentVersionService documentVerService,
            IBackgroundJobManager backgroundJobManager,IOptionsSnapshot<ModelFileOption> modelOption,          
            IOptionsSnapshot<AbpDbConnectionOptions> dbConnectionOptions,IHttpClientFactory httpClient,
            IRepository<Document, Guid> _DocumentRepository,IRepository<DocumentVersion, Guid> _DocumentVerRepository,
            IRepository<DocumentLog, Guid> _DocLogRepository,ILogsService logsService,ILogger<DocumentHandleAppService> logger )
        {
            DocumentService = documentService;
            DocumentVerService = documentVerService;
            DocumentVerRepository = _DocumentVerRepository;
            DocLogRepository = _DocLogRepository;
            BackgroundJobManager = backgroundJobManager;           
            ModelOption = modelOption.Value;          
            DbConnectionOptions = dbConnectionOptions.Value;
            HttpClient = httpClient;
            Client = new HttpHelp(HttpClient);
            _logsService = logsService;
            DocumentRepository = _DocumentRepository;
            _logger = logger;                  
        }

        #region 查询模型轻量化状态、同步数据

        /// <summary>
        /// 模型查询轻量化状态
        /// </summary>       
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task GetQueryModelInfoAsync()
        {
            var dtos = await DocumentVerService.GetLightweightModelAsync();
            foreach (var dto in dtos)
            {
                var requestUrl = $"{ModelOption.Url}/api/app/model/query-model-info?LightweightName="+dto.ModelName;
                if (dto.DocumentType==DocTypeEnum.GIS)
                {
                    requestUrl = $"{ModelOption.Url}/api/app/gismodel/QueryModelInfo?LightweightName=" + dto.ModelName;
                }
                var result = await Client.HttpPostAsync(requestUrl, null,ModelOption.Token);
                _logger.LogInformation(requestUrl + "BIM/GIS轻量化状态:"+result);
                dynamic json = result.FromJson<dynamic>();               
                if (json?.code == 1)
                {
                    if (json.datas == null)
                    {
                        continue;
                    }
                    int status = json.datas[0]?.status;
                    if (status > 0 && status < 100)//正在轻量化
                    {
                        await DocumentVerService.SyncModelStatus(dto.Id, DocStatusEnum.Running, string.Empty);
                    }
                    else if (status == 100)//轻量化成功
                    {
                        await DocumentVerService.SyncModelStatus(dto.Id, DocStatusEnum.Succeed,string.Empty);
                        await GetSyncDBToDatabaseAsync(dto.ModelName, dto.DocumentType);
                    }
                    else if (status < 0 && status > -100)//轻量化失败
                    {
                        await DocumentVerService.SyncModelStatus(dto.Id, DocStatusEnum.Failure, json.datas[0]?.TranscodingInfo);                         
                    }                    
                }                 
            }
        }

        /// <summary>
        /// 查询CAD轻量化状态
        /// </summary>        
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task GetQueryCADInfoAsync()
        {
            var dtos = await DocumentVerService.GetLightweightCADAsync();
            foreach (var dto in dtos)
            {
                var requestUrl = $"{ModelOption.Url}/api/app/cad-drawing/query-model-info?LightweightName="+dto.ModelName;
                //var dicParam = new Dictionary<string, string>();
                //dicParam.Add("LightweightName", dto.ModelName);
                var result = await Client.HttpPostAsync(requestUrl, null,ModelOption.Token);
                _logger.LogInformation(requestUrl + "CAD图纸轻量化状态:" + result);
                dynamic json = result.FromJson<dynamic>();

                if (json?.code == 1)
                {
                    if (json.datas == null)
                    {
                        continue;
                    }
                    int status = json.datas[0]?.status;
                    if (status > 0 && status < 100)//正在轻量化
                    {
                        await DocumentVerService.SyncModelStatus(dto.Id, DocStatusEnum.Running,string.Empty);
                    }
                    else if (status == 100)//轻量化成功
                    {
                        await DocumentVerService.SyncModelStatus(dto.Id, DocStatusEnum.Succeed, string.Empty);                        
                        await GetSyncDBToDatabaseAsync(dto.ModelName, dto.DocumentType);
                    }
                    else if (status < 0 && status > -100)//轻量化失败
                    {
                        await DocumentVerService.SyncModelStatus(dto.Id, DocStatusEnum.Failure, json.datas[0]?.TranscodingInfo);
                    }                    
                }                 
            }
        }


        /// <summary>
        /// 同步数据库文件
        /// </summary>
        /// <param name="lightweightName"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [RemoteService(false)]
        public async Task GetSyncDBToDatabaseAsync(string lightweightName, DocTypeEnum docType)
        {
            if (lightweightName.IsNotNullOrEmpty())
            {
                DesignDbContextHelper.CreateTables(lightweightName);//创建模型分表
                string modelConnectionString = DbConnectionOptions.ConnectionStrings.GetValueOrDefault("Model");
                List<string> connectionString = modelConnectionString.SplitRemoveEmptyToList(';');
                var token =ModelOption.Token;
                var requestUrl = $"{ModelOption.Url}/api/app/model/syncdbtodatabase";
                if (docType.Equals(DocTypeEnum.CAD))
                {                    
                    requestUrl = $"{ModelOption.Url}/api/app/cad-drawing/syncdbtodatabase";
                }
                Dictionary<string, string> parameters = new();
                parameters.Add("LightweightName", lightweightName);
                parameters.Add("DatabaseCategory", "2");//1:SqlServer 2:MySql
                parameters.Add("Server", connectionString.Find(x => x.Contains("Server")).Split('=')[1]);
                parameters.Add("Database", connectionString.Find(x => x.Contains("Database")).Split('=')[1]);
                parameters.Add("Uid", connectionString.Find(x => x.Contains("Uid")).Split('=')[1]);
                parameters.Add("PassWord", connectionString.Find(x => x.Contains("Pwd")).Split('=')[1]);
                parameters.Add("Port", connectionString.Find(x => x.Contains("Port")).Split('=')[1]);
                parameters.Add("NewModelName", "");
                parameters.Add("DeleteOriginalData", "true");
                parameters.Add("CallBackUrl", "");
                parameters.Add("SubTableSuffix", $"_{lightweightName}");//分表后缀名
                parameters.Add("DatabaseEngine", "0");//数据库引擎,只针对Mysql, 0为MyISAM，1为InnoDB（阿里云服务器中引擎需要配置为InnoDB）
                parameters.Add("CreateTableIndex", "1");//是否给新建表(属性表(model_property)的构件字段(externalId)和模型名称字段（modelName）;结构表(model_tree,model_type,model_group)的glid和pGlid)加索引：0：不加，1：加
                var result= await Client.HttpPostFormDataAsync(requestUrl, parameters, token);
                _logger.LogInformation(requestUrl + "同步数据" + result);
            }
        }
        #endregion


        #region 文件资料分块上传
        /// <summary>
        /// 文件上传-续传
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="parentId"></param>
        /// <param name="fileName"></param>
        /// <param name="index"></param>
        /// <param name="hashName"></param>
        /// <returns></returns>
        [DisableRequestSizeLimit]
        public async Task<bool> BreakpointContinuationAsync([FromForm] IFormFile chunk, string parentId, string fileName, int index, string hashName)
        {
            try
            {
                string rootPath = AppContext.BaseDirectory;
                string uploadDir = Path.Combine(rootPath, "uploads", "temp", hashName);//上传路径
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string uploadPath = Path.Combine(uploadDir, fileName);
                await Upload(chunk, uploadPath);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 文件上传-合并
        /// </summary>
        /// <param name="hashName"></param>
        /// <param name="parentId"></param>
        /// <param name="fileName"></param>
        /// <param name="suffixName"></param>
        /// <param name="id"></param>
        /// <param name="projectId"></param>
        /// <param name="fileSize"></param>
        /// <param name="openStatus"></param>
        /// <param name="userId"></param>
        /// <param name="isCurrent"></param>
        /// <param name="remark">新版说明</param>
        /// <returns></returns>
        public async Task<bool> MergeAsync(string hashName, string parentId, string fileName, string suffixName, Guid id, Guid projectId, int fileSize, OpenStatusEnum openStatus, string userId, bool isCurrent,string remark="")
        {
            try
            {
                var blobName = Guid.NewGuid().ToString();
                var ohterFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedUploadFormats))?.Split(",", StringSplitOptions.RemoveEmptyEntries);//其它文件限制
                var extName = Path.GetExtension(fileName).ToLower();//文件类型名
                string baseDir = AppContext.BaseDirectory;
                string uploadAbsolutePath = Path.Combine(baseDir, "uploads", "temp", hashName);
                string saveFolder = Path.Combine(baseDir, "uploads");  //文件保存文件夹
                string savePath = Path.Combine(baseDir, "uploads", blobName + extName); //文件保存完整路径
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }
                if (Directory.Exists(uploadAbsolutePath))
                {
                    var files = Directory.GetFiles(uploadAbsolutePath);
                    using (var saveStream = new FileStream(savePath, FileMode.Create))
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            using (var stream = new FileStream(Path.Combine(uploadAbsolutePath, hashName + "_" + i + "." + suffixName), FileMode.Open))
                            {
                                var byteArr = new byte[stream.Length];
                                await stream.ReadAsync(byteArr, 0, byteArr.Length);
                                await saveStream.WriteAsync(byteArr, 0, byteArr.Length);
                            }
                        }

                        //更新数据表                       
                        await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "资料列表", Name = "资料上传", Content = $"上传资料，资料名称[{fileName}]" });

                        //移动word/excel文件到wwwroot文件夹中
                        if (new string[4] { ".docx", ".doc", ".xls", ".xlsx" }.Contains(extName))
                        {
                            if (fileSize / 1024 / 1024 >= 20 && new string[2] { ".docx", ".doc" }.Contains(extName))
                            {
                                string wwwrootPath = Path.Combine(baseDir, "wwwroot", "uploads", blobName);
                                if (!Directory.Exists(wwwrootPath))
                                {
                                    Directory.CreateDirectory(wwwrootPath);
                                }
                                //异步上传并拆分word文件
                                await BackgroundJobManager.EnqueueAsync(new WordUploadAags
                                {
                                    DirPath = wwwrootPath,
                                    FilePath = savePath
                                });
                            }
                            else
                            {
                                string wwwrootPath = Path.Combine(baseDir, "wwwroot", "uploads");
                                if (!Directory.Exists(wwwrootPath))
                                {
                                    Directory.CreateDirectory(wwwrootPath);
                                }
                                var file = new FileInfo(savePath);
                                if (file.Exists)
                                {
                                    file.CopyTo(Path.Combine(wwwrootPath, blobName + extName));
                                }
                            }
                        }
                    }
                    //删除temp目录中的上传文件夹
                    _ = Task.Factory.StartNew(() =>
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(uploadAbsolutePath);
                        directoryInfo.Delete(true);
                    });
                }
                return true;
            }
            catch (Exception e)
            {               
                return false;
            }

        }

        #endregion


        /********以下代码作废********/



        #region 模型/cad首次上传
        /// <summary>
        /// 模型文件上传
        /// </summary>
        /// <param name="lightweightName"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSize"></param>       
        /// <param name="id"></param>
        /// <param name="openStatus"></param>      
        /// <param name="isCurrent"></param>
        /// <param name="docType"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public async Task<DocumentVersionDto> CreateUploadModelAsync(string lightweightName, string fileName, int fileSize, Guid id, OpenStatusEnum openStatus, bool isCurrent, DocTypeEnum docType=DocTypeEnum.Model,string format="")
        {
            DocumentVersionDto dtomod = new DocumentVersionDto();
            try
            {
                //var modelFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedModelUploadFormats))?.Split(",", StringSplitOptions.RemoveEmptyEntries);//模型文件限制
                //var cadFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedCADUploadFormats))?.Split(",", StringSplitOptions.RemoveEmptyEntries);//CAD文件限制
                //var extName = Path.GetExtension(fileName).ToLower();//文件类型名
                //if (modelFormats.Contains(extName))
                //{
                //    dtomod = await this.ProcessModelFile(lightweightName, fileName, fileSize, projectId, id, openStatus, userId.SplitToGuid(), true);
                //    await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "图模列表", Name = "图模上传", Content = $"上传图模，图模名称[{fileName}]" });
                //}
                //else if (cadFormats.Contains(extName))
                //{
                //    dtomod = await this.ProcessCADFile(lightweightName, fileName, fileSize, projectId, id, openStatus, userId.SplitToGuid(), true);
                //    await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "图模列表", Name = "图模上传", Content = $"上传图模，图模名称[{fileName}]" });
                //} 

                double docVer = 1.0;
                DocumentVersionCreateUpdateDto dto = new();                
                //dto.DocumentName = fileName;
                dto.ModelName = lightweightName;
                
                dto.Size = fileSize;
                dto.VersionNo = docVer;
               // dto.Status = DocStatusEnum.Await;
              //  dto.DocumentType = docType;
                dto.IsCurrent = isCurrent;
               // dto.ModelFormat = format;
                if (dto.IsCurrent)
                {
                    //dto.Document = ObjectMapper.Map<DocumentVersionCreateUpdateDto, DocumentCreateUpdateDto>(dto);
                    //dto.Document.ProjectFolderId = id;
                    //dto.Document.DocumentType = docType;
                    //dto.Document.VersionNo = docVer;                     
                    //dto.Document.DocumentLog = new Dtos.DocumentLog.DocumentLogCreateUpdateDto() { Remark = "文件已上传" };
                   // dto.Document.ModelFormat = format;
                }
                var entity = await DocumentVerService.CreateAsync(dto);
                await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "图模列表", Name = "图模上传", Content = $"上传图模，图模名称[{fileName}]" });
                return entity;
            }
            catch
            {
            }
            return dtomod;
        }

        //protected async Task<DocumentVerDto> ProcessCADFile(string lightweightName, string fileName, int fileSize, Guid projectId, Guid? parentId, OpenStatusEnum openStatus, Guid[] userId, bool isCurrent = false)
        //{
        //    string docVer = "V1.0";
        //    DocumentVerCreateUpdateDto dto = new();
        //    dto.ProjectId = projectId;
        //    dto.DocName = Path.GetFileNameWithoutExtension(fileName);

        //    dto.ModelName = lightweightName;
        //    dto.Suffix = Path.GetExtension(fileName);//文件名
        //    dto.DocSize = fileSize;
        //    dto.DocVer = docVer;
        //    dto.DocStatus = DocStatusEnum.Await;
        //    dto.DocType = DocTypeEnum.CAD;
        //    dto.IsCurrent = isCurrent;
        //    if (dto.IsCurrent)
        //    {
        //        dto.Document = ObjectMapper.Map<DocumentVerCreateUpdateDto, DocumentCreateUpdateDto>(dto);
        //        dto.Document.ParentId = parentId;
        //        dto.Document.DocType = DocTypeEnum.CAD;
        //        dto.Document.DocVer = docVer;
        //        dto.Document.OpenStatus = openStatus;                 
        //        dto.Document.DocumentLog = new Dtos.DocumentLog.DocumentLogCreateUpdateDto() { Remark = "文件已上传" };
        //    }
        //    var entity = await DocumentVerService.CreateAsync(dto);            
        //    return entity;
        //}

        /// <summary>
        /// 处理模型文件上传
        /// </summary>
        /// <param name="lightweightName"></param>
        /// <param name="fileName"></param>
        /// <param name="size"></param>        
        /// <param name="projectId"></param>
        /// <param name="parentId"></param>
        /// <param name="openStatus"></param>
        /// <param name="userId"></param>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        //protected async Task<DocumentVerDto> ProcessModelFile(string lightweightName, string fileName, int size, Guid projectId, Guid parentId, OpenStatusEnum openStatus, Guid[] userId, bool isCurrent = false)
        //{
        //    string docVer = "V1.0";

        //    DocumentVerCreateUpdateDto dto = new();
        //    dto.ProjectId = projectId;
        //    dto.DocName = Path.GetFileNameWithoutExtension(fileName);
        //    dto.ModelName = lightweightName;
        //    dto.Suffix = Path.GetExtension(fileName);//文件名
        //    dto.DocSize = size;
        //    dto.DocVer = docVer;
        //    dto.DocStatus = DocStatusEnum.Await;
        //    dto.DocType = DocTypeEnum.Model;
        //    dto.IsCurrent = isCurrent;
        //    if (dto.IsCurrent)
        //    {
        //        dto.Document = ObjectMapper.Map<DocumentVerCreateUpdateDto, DocumentCreateUpdateDto>(dto);
        //        dto.Document.ParentId = parentId;
        //        dto.Document.DocType = DocTypeEnum.Model;
        //        dto.Document.OpenStatus = openStatus;
        //        dto.Document.DocVer = docVer;
        //        dto.Document.DocumentLog = new Dtos.DocumentLog.DocumentLogCreateUpdateDto() { Remark = "文件已上传" };
        //    }
        //    var entity = await DocumentVerService.CreateAsync(dto);
        //    var docEntity = await DocumentRepository.GetAsync(entity.DocId);
        //    docEntity.LastModificationTime = DateTime.Now;
        //    await DocumentRepository.UpdateAsync(docEntity);
        //    return entity;
        //}
        #endregion



        #region 资料分块下载
        /// <summary>
        /// 获取下载文件大小
        /// </summary>
        /// <param name="fileName">带扩展名</param>
        /// <returns></returns>
        public async Task<int> GetDownloadFileSize(string fileName)
        {
            int totalSize = 0;
            string rootPath = AppContext.BaseDirectory; 
            string dirPath = Path.Combine(rootPath, "uploads");//路径
            var filePath = Path.Combine(dirPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    totalSize = (int)fs.Length;
                }
            }           

            return totalSize;
        }

        /// <summary>
        /// 下载资料
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="leftPosition"></param>
        /// <param name="rightPosition"></param>
        /// <param name="sectionSize"></param>
        /// <returns></returns>
        public async Task<FileResult> CreateDownloadFilesAsync(string fileName, int leftPosition, int rightPosition, int sectionSize)
        {
            string rootPath = AppContext.BaseDirectory; // 根路径
            byte[] byteArray = new byte[sectionSize];
            string dirPath = Path.Combine(rootPath, "uploads");//路径
            var filePath = Path.Combine(dirPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek(leftPosition, SeekOrigin.Begin);
                    await fs.ReadAsync(byteArray, 0, (rightPosition - leftPosition + 1));
                }
            }
            return new FileContentResult(byteArray, DesignMimeTypes.GetMIMEType(fileName));
        }


        
        #endregion





        #region 模型、cad上传新版本
        /// <summary>
        /// 模型/cad上传新版本
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="lightweightName"></param>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSize"></param>
        /// <param name="isCurrent"></param>
        /// <param name="remark">新版说明</param>
        /// <returns></returns>
        [DisableRequestSizeLimit]
        public async Task<DocumentVersionDto> CreateUploadBimOrCadAsync(Guid docId, string lightweightName, string userId, string fileName, int fileSize, string postilIds, bool isCurrent=false,string remark="" )
        {
            DocumentVersionDto dtomod = new DocumentVersionDto();
            try
            {
                var documentDto = await DocumentService.GetAsync(docId);
                var modelFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedModelUploadFormats))?.Split(",", StringSplitOptions.RemoveEmptyEntries);//模型文件限制
                var cadFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedCADUploadFormats))?.Split(",", StringSplitOptions.RemoveEmptyEntries);//CAD文件限制

                var extName = Path.GetExtension(fileName).ToLower();//文件类型名

                if (modelFormats.Contains(extName))
                {                    
                    dtomod = await this.ProcessDetailBimOrCadFile(lightweightName, fileName, fileSize,documentDto.Id, documentDto.IsPublic, DocTypeEnum.Model,  isCurrent,remark);
                }
                else if (cadFormats.Contains(extName))
                {                  
                    dtomod = await this.ProcessDetailBimOrCadFile(lightweightName, fileName, fileSize,documentDto.Id, documentDto.IsPublic, DocTypeEnum.CAD,  isCurrent,remark);
                }
                else
                {
                    throw new UserFriendlyException("不支持的文件类型！");
                }
            }
            catch
            {
            }
            return dtomod;
        }

        protected async Task<DocumentVersionDto> ProcessDetailBimOrCadFile(string lightweightName, string fileName, int fileSize,Guid documentId, bool OpenStatus, DocTypeEnum docType, bool isCurrent = false,string remark="")
        {        
            var document=await DocumentRepository.GetAsync(documentId);
            var verlist = await DocumentVerRepository.GetListAsync(s => s.DocumentId == documentId);
            var vermodel = verlist.OrderByDescending(s => s.CreationTime).First();
            double docVer = 1.0;
             
                docVer += 0.1;                
             
            #region 文件数据入库和调用文件上传接口
            DocumentVersionCreateUpdateDto dto = new();             
            //dto.DocumentName = fileName;
            dto.DocumentId =documentId;            
            dto.Size = fileSize;
            dto.VersionNo = docVer;
            //dto.Status = DocStatusEnum.Await;
            //dto.DocumentType = docType;
            dto.ModelName = lightweightName;
            dto.IsCurrent = isCurrent;
            dto.Remark = remark;
            //dto.ModelFormat = document.ModelFormat;
            //if (dto.IsCurrent)
            //{
            //    dto.Document = ObjectMapper.Map<DocumentVerCreateUpdateDto, DocumentCreateUpdateDto>(dto);
            //    dto.Document.ParentId = parentId;
            //    dto.Document.DocType = DocTypeEnum.File;
            //    dto.Document.DocVer = docVer;
            //    dto.Document.OpenStatus = OpenStatus;
            //    dto.Document.UserId = userId;
            //}

            Document docModel = null;
            DocumentVersion oldEntity = null;
            if (dto.IsCurrent)
            {
                docModel = await DocumentRepository.GetAsync(documentId);
                //先获取当前设置的版本信息，才能进行添加操作
                oldEntity = await DocumentVerRepository.FirstOrDefaultAsync(o => o.IsCurrent && o.DocumentId == documentId);
                //设置版本
                
                docModel.ModelName = dto.ModelName;
                docModel.Size = dto.Size;
                docModel.VersionNo = dto.VersionNo;
               // docModel.Status = dto.Status;
                if (oldEntity != null)
                {
                    oldEntity.IsCurrent = false;
                    docModel.DocumentVersion.Add(oldEntity);
                }
            }
            var entity = await DocumentVerService.CreateAsync(dto);
            //版本信息保存成功后，提交版本切换相关数据
            if (dto.IsCurrent) await DocumentRepository.UpdateAsync(docModel);

            var docEntity = await DocumentRepository.GetAsync(entity.DocumentId);
            docEntity.LastModificationTime = DateTime.Now;
            await DocumentRepository.UpdateAsync(docEntity);
            #endregion            
            return entity;
        }
        #endregion

        private async Task Upload(IFormFile chunk, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await chunk.CopyToAsync(stream);
            }
        }
    }
}
