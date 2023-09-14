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
using Glendale.Design.Dtos.DocumentConfig;

namespace Glendale.Design.Services
{
    [Authorize]
    public class DocumentConfigService : CrudAppService<DocumentConfig, DocumentConfigDto, DocumentConfigListDto, Guid, DocumentConfigInput, DocumentConfigCreateUpdateDto, DocumentConfigCreateUpdateDto>, IDocumentConfigService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        public DocumentConfigService(IRepository<DocumentConfig, Guid> repository) : base(repository)
        {
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>        
        public override async Task<DocumentConfigDto> CreateAsync(DocumentConfigCreateUpdateDto input)
        {
            return await base.CreateAsync(input);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<DocumentConfigDto> UpdateAsync(Guid id, DocumentConfigCreateUpdateDto input)
        {
            return await base.UpdateAsync(id, input);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async override Task<PagedResultDto<DocumentConfigListDto>> GetListAsync(DocumentConfigInput input)
        {
            var Query = Repository
                .WhereIf(input.DocumentVerId.HasValue, s => s.DocumentVerId == input.DocumentVerId.Value)
                .WhereIf(input.DocumentId.HasValue, s => s.DocumentId == input.DocumentId.Value)
                .WhereIf(input.VersionNo.HasValue, s => s.VersionNo == input.VersionNo.Value)
                .WhereIf(!string.IsNullOrEmpty(input.ModelName), s => s.ModelName == input.ModelName);
                
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var dtos = ObjectMapper.Map<List<DocumentConfig>, List<DocumentConfigListDto>>(entitys);                    
            return new PagedResultDto<DocumentConfigListDto>(totalCount, dtos);
        }
        

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);
        }
        

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task<DocumentConfigDto> GetAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
    }
}