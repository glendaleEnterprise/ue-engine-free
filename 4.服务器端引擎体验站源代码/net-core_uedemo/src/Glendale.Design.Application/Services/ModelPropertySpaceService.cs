using Glendale.Design.Dtos.ModelPropertySpace;
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
    /// 模型属性表
    /// </summary>
    [AllowAnonymous]
    public class ModelPropertySpaceAppService : CrudAppService<ModelPropertySpace, ModelPropertySpaceDto, ModelPropertySpaceListDto, int, GetListModelPropertySpaceInput,
                            ModelPropertySpaceCreateUpdateDto, ModelPropertySpaceCreateUpdateDto>, IModelPropertySpaceAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        public ModelPropertySpaceAppService(IRepository<ModelPropertySpace, int> repository)
            : base(repository)
        {
        }
        [RemoteService(false)]
        public override Task<ModelPropertySpaceDto> CreateAsync(ModelPropertySpaceCreateUpdateDto input)
        {
            return base.CreateAsync(input);
        }

        [RemoteService(false)]
        public override Task<ModelPropertySpaceDto> UpdateAsync(int id, ModelPropertySpaceCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        [RemoteService(false)]
        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }

        [RemoteService(false)]
        public override Task<ModelPropertySpaceDto> GetAsync(int id)
        {
            return base.GetAsync(id);
        }

        [RemoteService(false)]
        public override Task<PagedResultDto<ModelPropertySpaceListDto>> GetListAsync(GetListModelPropertySpaceInput input)
        {
            return base.GetListAsync(input);
        }

        //protected override async Task<IQueryable<ModelPropertySpace>> CreateFilteredQueryAsync(GetListModelPropertySpaceInput input)
        //{
        //    var Query = await base.CreateFilteredQueryAsync(input);
        //    return Query.WhereIf(!input.ExternalId.IsNullOrEmpty(), o => o.ExternalId == input.ExternalId);
        //}

        /// <summary>
        /// 获取构件所有属性
        /// </summary>
        /// <param name="externalId">组件id</param>
        /// <param name="modelName">模型名称</param>
        /// <returns></returns>
        [HttpGet("/api/app/model-Property-Space/GetProperty")]
        [AllowAnonymous]
        public async Task<List<ModelPropertySpaceDto>> GetModelPropertySpace(string externalId, string modelName)
        {
            try { 
                var Query = await base.CreateFilteredQueryAsync(new GetListModelPropertySpaceInput());
                Query = Query.Where(o => o.ExternalId == externalId && o.ModelName == modelName);
                var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
                var list = ObjectMapper.Map<IEnumerable<ModelPropertySpace>, List<ModelPropertySpaceDto>>(entitys);
                return list;
            }
            catch
            {
                return new List<ModelPropertySpaceDto>();
            }
        }
    }
}