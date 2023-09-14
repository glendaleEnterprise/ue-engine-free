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
    /// 合摸压平接口
    /// </summary>
    public class CombineFlattenAppService : CrudAppService<CombineFlatten, CombineFlattenDto, CombineFlattenListDto, Guid, GetListCombineFlattenInput, CreateCombineFlattenDto, CreateCombineFlattenDto>, ICombineFlattenAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        private readonly ILogsService _logsService;

        private readonly IRepository<CombineLog, Guid> CombineLogRepository;
        public CombineFlattenAppService(IRepository<CombineFlatten, Guid> repository, ILogsService logsService) : base(repository)
        {
            _logsService = logsService;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<CombineFlattenDto> CreateAsync(CreateCombineFlattenDto input)
        {
            await CheckCreatePolicyAsync();
            var entity = await MapToEntityAsync(input);

            entity = await Repository.InsertAsync(entity, autoSave: true);
            await _logsService.CreateAsync(new Dtos.LogsCreateUpdateDto{ ModuleName = "压平列表", Name = "新增", Content = $"新增合模压平[{input.FlattenName}]" });
            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [RemoteService(false)]
        public override async Task<CombineFlattenDto> UpdateAsync(Guid id, CreateCombineFlattenDto input)
        {
            return await base.UpdateAsync(id, input);
        }


        /// <summary>
        /// 查询【带分页】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<CombineFlattenListDto>> GetListAsync(GetListCombineFlattenInput input)
        {
            //var listFolderId = new List<Guid>();
            //var folders = await _documentService.GetChildTree(input.FolderId, Enums.DocTypeEnum.Folder);
            //if (folders != null)
            //{
            //    listFolderId = folders.Select(p => p.Id).ToList();
            //}
            //listFolderId.Add(input.FolderId);


            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query.Where(p => p.CombineId == input.CombineId)
                .WhereIf(!input.FlattenName.IsNullOrEmpty(), o => o.FlattenName.StartsWith(input.FlattenName))
                .WhereIf(input.FlattenType.HasValue, o => o.FlattenType == input.FlattenType.Value);

            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<CombineFlatten>, IReadOnlyList<CombineFlattenListDto>>(entitys);          
            return new PagedResultDto<CombineFlattenListDto>(totalCount, list);
        }


        /// <summary>
        /// 查询【不带分页】
        /// </summary>
        /// <param name="CombineId"></param>
        /// <param name="FlattenName"></param>
        /// <returns></returns>
        //[HttpGet("/api/app/combine-flatten/GetAllListAsync")]
        public async Task<List<CombineFlattenListDto>> GetAllListAsync(Guid CombineId, string FlattenName)
        {
            var Query = await base.CreateFilteredQueryAsync(new GetListCombineFlattenInput());
            Query = Query.Where(o => o.CombineId == CombineId)
                .WhereIf(!FlattenName.IsNullOrEmpty(), o => o.FlattenName == FlattenName);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            return ObjectMapper.Map<IEnumerable<CombineFlatten>, List<CombineFlattenListDto>>(entitys);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<CombineFlattenDto> GetAsync(Guid id)
        {
            var entity = await base.Repository.FirstOrDefaultAsync(o => o.Id == id);    
           return await MapToGetOutputDtoAsync(entity);
        }
    }
}
