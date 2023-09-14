using Glendale.Design.Dtos;
using Glendale.Design.Dtos.Label;
using Glendale.Design.Enums;
using Glendale.Design.Jobs;
using Glendale.Design.Models;
using Glendale.Design.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    public class LabelService : CrudAppService<Label,LabelDto,LabelListDto,Guid,LabelInput,LabelCreateUpdateDto,LabelCreateUpdateDto>,ILabelService
    {
        private readonly IRepository<Label, Guid> _labelRepository;    
        private readonly IRepository<Models.File, Guid> _fileRepository;
        private readonly ILogsService _logsService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public LabelService(IRepository<Label, Guid> repository, ILogsService logsService, IBackgroundJobManager backgroundJobManager, IRepository<Models.File, Guid> fileRepository)
            : base(repository)
        {
            _labelRepository = repository;
            _logsService = logsService;
            _backgroundJobManager = backgroundJobManager;
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Task<LabelDto> CreateAsync(LabelCreateUpdateDto input)
        {
            return base.CreateAsync(input);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async override Task<PagedResultDto<LabelListDto>> GetListAsync(LabelInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);

            Query = Query
                .WhereIf(!string.IsNullOrEmpty(input.modelId), s => s.ModelId == input.modelId);
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);

            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<Label>, IReadOnlyList<LabelListDto>>(entitys);

            var blobarry = list.Select(s => s.BlobName).ToArray();
            var flist = await _fileRepository.Where(s => blobarry.Contains(s.BlobName)).ToListAsync();
            foreach (var item in list)
            {
                var file = flist.Where(s => s.BlobName == item.BlobName).FirstOrDefault();
                if (file == null) continue;
                item.ByteSize = file.ByteSize;
                item.FileName = file.FileName;
                item.Suffix = file.Extension;
            }
            return new PagedResultDto<LabelListDto>(totalCount, list);          
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
        /// 文件上传-续传
        /// </summary>
        /// <param name="chunk"></param>      
        /// <param name="fileName"></param>
        /// <param name="index"></param>
        /// <param name="hashName"></param>
        /// <returns></returns>
        [DisableRequestSizeLimit]
        public async Task<bool> BreakpointContinuationAsync([FromForm] IFormFile chunk, string fileName, int index, string hashName)
        {
            try
            {
                string rootPath = AppContext.BaseDirectory;
                string uploadDir = Path.Combine(rootPath, "uploads", "temp", hashName);//上传路径
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                string uploadPath = Path.Combine(uploadDir, fileName);
                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    await chunk.CopyToAsync(stream);
                }                
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
        /// <param name="fileName"></param>        
        /// <param name="projectId"></param>
        /// <param name="fileSize"></param> 
        /// <param name="modelId"></param>
        /// <param name="labelType"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<bool> MergeAsync(string hashName, string fileName, int fileSize,Guid projectId,string modelId, SceneTypeEnum sceneType, string position)
        {
            try
            {
                var blobName = Guid.NewGuid().ToString();
                var ohterFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedUploadFormats))?.Split(",", StringSplitOptions.RemoveEmptyEntries);//其它文件限制
                var extName = Path.GetExtension(fileName);//文件类型名
                string baseDir = AppContext.BaseDirectory;
                string uploadAbsolutePath = Path.Combine(baseDir, "uploads", "temp", hashName);
                string saveFolder = Path.Combine(baseDir, "uploads");  //文件保存文件夹
                string savePath = Path.Combine(baseDir, "uploads", blobName+ extName);
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
                            using (var stream = new FileStream(Path.Combine(uploadAbsolutePath, hashName + "_" + i + extName), FileMode.Open))
                            {
                                var byteArr = new byte[stream.Length];
                                await stream.ReadAsync(byteArr, 0, byteArr.Length);
                                await saveStream.WriteAsync(byteArr, 0, byteArr.Length);
                            }
                        }
                        //合并完成，保存到数据库
                        await _labelRepository.InsertAsync(new Label
                        {
                            BlobName = blobName,                                                      
                            LabelName =fileName.Substring(0,fileName.Length- extName.Length),
                            ModelId = modelId,
                            Position = position,
                            ProjectId = projectId,
                            SceneType = sceneType, 
                        });
                        

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
                                await _backgroundJobManager.EnqueueAsync(new WordUploadAags
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
            catch(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 文件链接分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //public async Task<PagedResultDto<DocumentAttachListDto>> GetAllByDocidAsync(DocumentAttachInput input)
        //{
        //    var total = 0;
        //    var query = from t in await _docAttachRepository.GetListAsync(p => p.Category == input.category && p.ModelId == input.docId.ToString())
        //                join o in await _fileRepository.GetListAsync()
        //                on t.FileId equals o.Id
        //                where 1 == 1
        //                orderby t.CreationTime descending
        //                select new DocumentAttachListDto
        //                {
        //                    Id = t.Id,
        //                    BlobName = o.BlobName,
        //                    ByteSize = o.ByteSize,
        //                    Category = t.Category,
        //                    CreationTime = t.CreationTime,
        //                    CreatorId = t.CreatorId,
        //                    FileId = t.FileId,
        //                    FileName = o.FileName,
        //                    IsEnable = t.IsEnable,
        //                    ModelId = t.ModelId,
        //                    Position = t.Position,
        //                    ProjectId = t.ProjectId
        //                };
        //    total = query.Count();
        //    var list = query.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
        //    return new PagedResultDto<DocumentAttachListDto>(total, list);            
        //}
     }
}
