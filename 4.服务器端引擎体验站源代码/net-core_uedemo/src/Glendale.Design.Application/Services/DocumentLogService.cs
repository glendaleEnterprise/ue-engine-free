using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glendale.Design.Dtos;
using Glendale.Design.Dtos.DocumentLog;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Linq;

namespace Glendale.Design.Services
{
    [Authorize]
    public class DocumentLogService : CrudAppService<DocumentLog,DocumentLogDto, DocumentLogListDto, Guid, GetListDocumentLogInput,DocumentLogCreateUpdateDto,DocumentLogCreateUpdateDto>,IDocumentLogService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly IIdentityUserRepository UserRepository;
        private readonly ILogsService _logsService;

        public DocumentLogService(IRepository<DocumentLog, Guid> repository, IIdentityUserRepository userRepository, ILogsService logsService) : base(repository)
        {
            UserRepository = userRepository;
            _logsService = logsService;
        }

        [RemoteService(false)]
        public override Task<DocumentLogDto> CreateAsync(DocumentLogCreateUpdateDto input)
        {
            return base.CreateAsync(input);
        }

        [RemoteService(false)]
        public override Task<DocumentLogDto> UpdateAsync(Guid id, DocumentLogCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        [RemoteService(false)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        [RemoteService(false)]
        public override Task<DocumentLogDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }


        protected override async Task<IQueryable<DocumentLog>> CreateFilteredQueryAsync(GetListDocumentLogInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Where(o => o.DocumentId == input.DocumentId)
                .WhereIf(!input.Keyword.IsNullOrEmpty(), o => o.Remark.Contains(input.Keyword));
            return Query;
        }


        /// <summary>
        /// 获取全部的日志
        /// </summary>
        /// <param name="docId">文档id</param>
        /// <returns></returns>
        public async Task<List<DocumentLogListDto>> GetAllByDocidAsync(Guid docId)
        {
           var entitys= await Repository.Include(x => x.Creator).Where(x => x.DocumentId.Equals(docId)).OrderBy(x=>x.CreationTime).ToListAsync();         
            return ObjectMapper.Map<IEnumerable<DocumentLog>, List<DocumentLogListDto>>(entitys);
        }

        /// <summary>
        /// 新增文档日志
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public async Task<DocumentLogDto> CreateLog(Guid docId, int logType)
        {
            string remark = string.Empty;
            switch (logType)
            {
                case 1:
                    remark = "添加文件";
                    break;
                case 2:
                    remark = "下载文件";
                    break;
                case 3:
                    remark = "更新文件版本";
                    break;
                case 4:
                    remark = "切换当前版本";
                    break;
                case 99:
                    remark = "删除文件版本";
                    break;
            }
            var dto = new DocumentLogCreateUpdateDto()
            {
                DocumentId = docId,
                Remark = remark,
            };
           return await base.CreateAsync(dto);
        }
    }
}
