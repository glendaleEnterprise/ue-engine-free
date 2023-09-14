using Glendale.Design.Dtos.ModelProperty;
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
    public class ModelPropertyAppService : CrudAppService<ModelProperty, ModelPropertyDto, ModelPropertyListDto, int, GetListModelPropertyInput,
                            ModelPropertyCreateUpdateDto, ModelPropertyCreateUpdateDto>, IModelPropertyAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        public ModelPropertyAppService(IRepository<ModelProperty, int> repository)
            : base(repository)
        {
        }
        [RemoteService(false)]
        public override Task<ModelPropertyDto> CreateAsync(ModelPropertyCreateUpdateDto input)
        {
            return base.CreateAsync(input);
        }

        [RemoteService(false)]
        public override Task<ModelPropertyDto> UpdateAsync(int id, ModelPropertyCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        [RemoteService(false)]
        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }

        [RemoteService(false)]
        public override Task<ModelPropertyDto> GetAsync(int id)
        {
            return base.GetAsync(id);
        }

        [RemoteService(false)]
        public override Task<PagedResultDto<ModelPropertyListDto>> GetListAsync(GetListModelPropertyInput input)
        {
            return base.GetListAsync(input);
        }

        //protected override async Task<IQueryable<ModelProperty>> CreateFilteredQueryAsync(GetListModelPropertyInput input)
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
        [HttpGet("/api/app/model-Property/GetProperty")]
        [AllowAnonymous]
        public async Task<List<ModelPropertyDto>> GetModelProperty(string externalId, string modelName)
        {
            try { 
                var Query = await base.CreateFilteredQueryAsync(new GetListModelPropertyInput());
                Query = Query.Where(o => o.ExternalId == externalId && o.ModelName == modelName);
                var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
                var list = ObjectMapper.Map<IEnumerable<ModelProperty>, List<ModelPropertyDto>>(entitys);
                return list;
            }
            catch
            {
                return new List<ModelPropertyDto>();
            }
        }
    }
}