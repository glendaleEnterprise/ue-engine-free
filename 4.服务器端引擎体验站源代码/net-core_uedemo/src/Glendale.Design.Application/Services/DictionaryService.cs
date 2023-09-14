using Glendale.Design.Dtos;
using Glendale.Design.Dtos.Dictionary;
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
    /// 基础数据
    /// </summary>
    [Authorize]
    public class DictionaryService : CrudAppService<Dictionary, DictionaryDto, DictionaryListDto, Guid, GetListDictionaryInput,DictionaryCreateUpdateDto, DictionaryCreateUpdateDto>, IDictionaryService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }        
        private readonly ILogsService _logsService;

        public DictionaryService(IRepository<Dictionary, Guid> repository, ILogsService logsService) : base(repository)
        {            
            _logsService = logsService;
        }
        protected override async Task<IQueryable<Dictionary>> CreateFilteredQueryAsync(GetListDictionaryInput input)
        {
            var Query = await base.CreateFilteredQueryAsync(input);
            return Query.Include(x => x.Children).ThenInclude(x => x.Children)
                .Where(p => p.ParentId == null)
                .WhereIf(input.Enable.HasValue, x => x.Enable.Equals(input.Enable))
                .WhereIf(!input.Keyword.IsNullOrEmpty(), t => t.Name.Contains(input.Keyword))
                .OrderBy(p => p.OrderIndex);
        }
        public override async Task<DictionaryDto> CreateAsync(DictionaryCreateUpdateDto input)
        {
            var entity = await MapToEntityAsync(input);
            if (input.ParentId == null)
            {
                entity.ParentId = null;
                entity.Parent = null;
            }
            else
            {
                var parentEntity = await Repository.GetAsync(Guid.Parse(input.ParentId.ToString()));
                entity.Parent = parentEntity;
            }
            await Repository.InsertAsync(entity, autoSave: true);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "基础数据维护", Name = "新增", Content = $"新增了数据：[{input.Name}]" });
            return await MapToGetOutputDtoAsync(entity);
        }
        public override async Task<DictionaryDto> UpdateAsync(Guid id, DictionaryCreateUpdateDto input)
        {
            await CheckUpdatePolicyAsync();
            var entity = await Repository.Include(x => x.Parent).Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            await MapToEntityAsync(input, entity);
            await Repository.UpdateAsync(entity, autoSave: true);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "基础数据维护", Name = "更新", Content = $"更新了数据：[{input.Name}]" });
            return await MapToGetOutputDtoAsync(entity);
        }

        public override Task<PagedResultDto<DictionaryListDto>> GetListAsync(GetListDictionaryInput input)
        {
            return base.GetListAsync(input);
        }
        public override Task<DictionaryDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DictionaryListDto>> GetAllListAsync()
        {
            var Query = await base.CreateFilteredQueryAsync(new GetListDictionaryInput());
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            return ObjectMapper.Map<IEnumerable<Dictionary>, IEnumerable<DictionaryListDto>>(entitys);
        }

        
        public async Task<IEnumerable<DictionaryListDto>> GetParentAsync(string parent)
        {
            var list = Repository.Include(x => x.Parent).Include(s => s.Children)
                .WhereIf(!parent.IsNullOrEmpty(), x => x.Parent.Value.Equals(parent))
                .OrderBy(x => x.OrderIndex).ToList();
            return await MapToGetListOutputDtosAsync(list);
        }

        public async Task<IEnumerable<DictionaryViewDto>> GetTreeAsync()
        {
            var Query = await base.CreateFilteredQueryAsync(new GetListDictionaryInput());
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            return ObjectMapper.Map<IEnumerable<Dictionary>, IEnumerable<DictionaryViewDto>>(entitys.Where(x => x.ParentId == null));
        }

        [RemoteService(false)]
        public async Task<DictionaryDto> GetByValueAsync(string value)
        {
            var entity = await Repository.Where(x => x.Value.Equals(value)).FirstOrDefaultAsync();
            return await MapToGetOutputDtoAsync(entity);
        }  
       
        [HttpPut]
        public async Task<bool> EnableAsync(Guid id)
        {
            var entity = await Repository.Include(x => x.Parent).Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();             
            if (entity != null)
            {
                entity.Enable = !entity.Enable;
                await Repository.UpdateAsync(entity, true);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
        public override async Task DeleteAsync(Guid id)
        {
            var entity =await Repository.GetAsync(id);            
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto { ModuleName = "基础数据维护", Name = "删除", Content = $"删除了数据：[{entity.Name}]" });
            await base.DeleteAsync(id);
        }
         
    }
}