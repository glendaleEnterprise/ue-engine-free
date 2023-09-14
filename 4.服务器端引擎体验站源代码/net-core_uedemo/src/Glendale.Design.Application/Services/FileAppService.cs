using Glendale.Design.Dtos.File;
using Glendale.Design.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using Volo.Abp.Http;
using Glendale.Design.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Volo.Abp.BackgroundJobs;
using Glendale.Design.Jobs;

namespace Glendale.Design.Services
{
    
    public class FileAppService : DesignAppService, IFileAppService
    {    
        protected IFileManager FileManager { get; }
        private readonly IBackgroundJobManager _backgroundJobManager;
        public FileAppService(IFileManager fileManager, IBackgroundJobManager backgroundJobManager)
        {
            FileManager = fileManager;
            _backgroundJobManager = backgroundJobManager;
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public virtual async Task<FileResult> GetAsync(string blobName)
        {
            Check.NotNullOrWhiteSpace(blobName, nameof(blobName));

            var file = await FileManager.FindByBlobNameAsync(blobName);
            var bytes = await FileManager.GetBlobAsync(blobName);

            return new FileContentResult(bytes ?? Array.Empty<byte>(), MimeTypes.GetByExtension(Path.GetExtension(file.FileName)));            
        }

        ///// <summary>
        ///// 浏览（分片上传文档）
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public virtual async Task<FileDto> GetDocumentAsync(Guid id)
        //{
        //    var entity = await DocRepository.GetAsync(id);
        //    string rootPath = AppContext.BaseDirectory; // 根路径
        //    string dirPath = Path.Combine(rootPath, "uploads", entity.ModelName);//路径
        //    var currentDocument = DocVerRepository.Where(p => p.DocId == id && p.IsCurrent == true).FirstOrDefault();
        //    string filePath = Path.Combine(dirPath, currentDocument.DocName);

        //    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //    long size = fs.Length;
        //    byte[] array = new byte[size];
        //    fs.Read(array, 0, array.Length);
        //    fs.Close();
        //    return new FileDto
        //    {
        //        Bytes = array,
        //        FileName = entity.DocName + entity.Suffix
        //    };
        //}

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public virtual async Task<string> CreateAsync([FromForm] IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);                
                    var dto = await FileManager.CreateAsync(file.FileName, stream.ToArray());
                    return dto.BlobName;
                }
            }
            catch (Exception e)
            {
                return String.Empty;
            }            
        }
        /// <summary>
        /// 文件上传-续传并自动完成合并
        /// </summary>
        /// <param name="file"></param>      
        /// <param name="fileName"></param>
        /// <param name="hashName"></param>
        /// <param name="chunks"></param>
        /// <param name="chunk"></param>
        /// <returns></returns>       
        public async Task<object> BreakpointContinuationAsync([FromForm] IFormFile file, string fileName, string hashName, int chunks, int chunk)
        {
            try
            {
                //验证上传文件后缀
                var fileFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedUploadFormats))?.Split(",", StringSplitOptions.RemoveEmptyEntries);//模型文件限制

                var extName = Path.GetExtension(fileName).ToLower();//文件类型名
                if (!fileFormats.Contains(extName))
                {
                    return new { code = 1, msg = "", sysmsg = "上传的文件已经存在" };
                }
                //追加获取到的文件流
                string rootPath = AppContext.BaseDirectory;
                string uploadDir = Path.Combine(rootPath, "uploads", "temp");//上传路径
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }
                //上传后的保存文件夹
                string uploadMoveDir = Path.Combine(rootPath, "wwwroot", "uploads");//上传路径
                if (!Directory.Exists(uploadMoveDir))
                {
                    Directory.CreateDirectory(uploadMoveDir);
                }
                var newfilePath = Path.Combine(uploadMoveDir, hashName + extName);
                if (System.IO.File.Exists(newfilePath))
                {
                    return new { code = 1, msg = "", sysmsg = "上传的文件已经存在" };
                }


                string uploadPath = Path.Combine(uploadDir, hashName + extName);

                string uploadTempPath = Path.Combine(uploadDir, Guid.NewGuid().ToString().Replace("-", "") + extName);
                using (var stream = new FileStream(uploadPath, FileMode.OpenOrCreate))
                {
                    using (var streamTemp = new FileStream(uploadTempPath, FileMode.Create))
                    {
                        //临时存储上传的文件
                        await file.CopyToAsync(streamTemp);

                        //将流指针移至开始
                        streamTemp.Position = 0;

                        //获取上传文件流
                        var byteArr = new byte[streamTemp.Length];
                        await streamTemp.ReadAsync(byteArr, 0, byteArr.Length);

                        //将流指针移至末尾
                        stream.Position = stream.Length;

                        //在流末尾写入新上传的文件流
                        await stream.WriteAsync(byteArr, 0, byteArr.Length);
                    }
                    System.IO.File.Delete(uploadTempPath);
                }

                if (chunks == chunk + 1)
                {
                    //移动word/excel文件到wwwroot文件夹中
                    if (new string[2] { ".docx", ".doc" }.Contains(extName))
                    {
                        string wwwrootPath = Path.Combine(rootPath, "wwwroot", "uploads", hashName);
                        if (!Directory.Exists(wwwrootPath))
                        {
                            Directory.CreateDirectory(wwwrootPath);
                        }
                        //异步上传并拆分word文件
                        await _backgroundJobManager.EnqueueAsync(new WordUploadAags
                        {
                            DirPath = wwwrootPath,
                            FilePath = uploadPath
                        });
                    }
                    System.IO.File.Move(uploadPath, newfilePath);
                }
                return new { code = 0, msg = "上传完成", sysmsg = "" };
            }
            catch (Exception ex)
            {
                return new { code = -1, msg = "上传失败", sysmsg = "底层出错：\r\n\r\nMessage：" + ex.Message + "\r\n\r\n" + ex.HelpLink + "\r\n\r\n" + ex.StackTrace };
            }
        }

        protected virtual async Task CheckFile(FileDto input)
        {
            if (input.Bytes.IsNullOrEmpty())
            {
                throw new AbpValidationException("文件不能为空！", new List<ValidationResult> { new ValidationResult("文件不能为空！", new[] { "Bytes" }) });
            }

            var allowedMaxFileSize = await SettingProvider.GetAsync<int>(DesignSettings.AllowedMaxFileSize);//kb
            var allowedUploadFormats = (await SettingProvider.GetOrNullAsync(DesignSettings.AllowedUploadFormats))
                ?.Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (input.Bytes.Length > allowedMaxFileSize * 1024)
            {
                throw new UserFriendlyException(L["Design.ExceedsTheMaximumSize", allowedMaxFileSize]);
            }

            if (allowedUploadFormats == null || !allowedUploadFormats.Contains(Path.GetExtension(input.FileName)))
            {
                throw new UserFriendlyException(L["Design.NotValidFormat", allowedUploadFormats.Join(",")]);
            }
        }

        /// <summary>
        /// word转Html
        /// </summary>
        /// <param name="fileName">带扩展名，如：602ace13-7336-4a48-b1f9-d7f7d7259572.docx</param>
        /// <param name="page"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<dynamic> GetWordHtmlAsync(string fileName, int? page)
        {
            string folder = fileName.Split(".")[0];
            string rootPath = AppContext.BaseDirectory;
            string uploadFile = Path.Combine(rootPath, "wwwroot", "uploads", fileName);//上传路径

            if (!System.IO.File.Exists(uploadFile))
            {
                throw new Volo.Abp.UserFriendlyException("文件不存在");
            }
            FileInfo file = new FileInfo(uploadFile);

            var total = 0;
            string result = "";

            if (file.Length / 1024 / 1024 >= 20)
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
                result = ce.office.extension.WordHelper.ToHtml(uploadFile);
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
            if (!System.IO.File.Exists(filePath))
            {
                throw new Volo.Abp.UserFriendlyException("文件不存在");
            }
            return Task.FromResult(ce.office.extension.ExcelHelper.ToHtml(filePath));
        }
    }
}
