﻿using Glendale.Design.Dtos.ModelType;
using Glendale.Design.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    /// 模型专业表
    /// </summary>
    [AllowAnonymous]
    public class ModelTypeAppService : CrudAppService<ModelType, ModelTypeDto, ModelTypeListDto, int, GetListModelTypeInput,
                            ModelTypeCreateUpdateDto, ModelTypeCreateUpdateDto>, IModelTypeAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        public ModelTypeAppService(IRepository<ModelType, int> repository)
            : base(repository)
        {
        }

        [RemoteService(false)]
        public override Task<ModelTypeDto> CreateAsync(ModelTypeCreateUpdateDto input)
        {
            return base.CreateAsync(input);
        }
        [RemoteService(false)]
        public override Task<ModelTypeDto> UpdateAsync(int id, ModelTypeCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }
        [RemoteService(false)]
        public override Task<PagedResultDto<ModelTypeListDto>> GetListAsync(GetListModelTypeInput input)
        {
            return base.GetListAsync(input);
        }
        [RemoteService(false)]
        public override Task<ModelTypeDto> GetAsync(int id)
        {
            return base.GetAsync(id);
        }
        [RemoteService(false)]
        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }

        /// <summary>
        /// 结构查询模型【带分页】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //public override async Task<PagedResultDto<ModelTypeListDto>> GetListAsync(GetListModelTypeInput input)
        //{
        //    var Query = await base.CreateFilteredQueryAsync(input);
        //    Query = Query
        //        .WhereIf(!input.GroupName.IsNullOrEmpty(), o => o.GroupName == input.GroupName)
        //        .WhereIf(!input.Glid.IsNullOrEmpty(), o => o.Glid == input.Glid)
        //        .WhereIf(!input.PGlid.IsNullOrEmpty(), o => o.PGlid == input.PGlid)
        //        .WhereIf(input.Level != null, o => o.Level == input.Level)
        //        .WhereIf(!input.Name.IsNullOrEmpty(), o => o.Name == input.Name)
        //        .WhereIf(!input.ExternalId.IsNullOrEmpty(), o => o.ExternalId == input.ExternalId);
        //    var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
        //    Query = ApplySorting(Query, input);
        //    Query = ApplyPaging(Query, input);
        //    var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
        //    var list = ObjectMapper.Map<IEnumerable<ModelType>, IReadOnlyList<ModelTypeListDto>>(entitys);
        //    return new PagedResultDto<ModelTypeListDto>(totalCount, list);
        //}
        /// <summary>
        /// 结构查询模型【不带分页】
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[HttpGet("/api/app/model-type/all")]
        public async Task<IReadOnlyList<ModelTypeListDto>> GetAll(GetListModelTypeInput input)
        {
            try
            {
                var Query = await base.CreateFilteredQueryAsync(new GetListModelTypeInput());
                Query = Query.Where(o => o.ModelName == input.ModelName)
                    .WhereIf(!input.GroupName.IsNullOrEmpty(), o => o.GroupName == input.GroupName)
                    .WhereIf(!input.Glid.IsNullOrEmpty(), o => o.Glid == input.Glid)
                    .WhereIf(!input.PGlid.IsNullOrEmpty(), o => o.PGlid == input.PGlid)
                    .WhereIf(input.Level != null, o => o.Level == input.Level)
                    .WhereIf(!input.Name.IsNullOrEmpty(), o => o.Name == input.Name)
                    .WhereIf(!input.ExternalId.IsNullOrEmpty(), o => o.ExternalId == input.ExternalId);
                var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
                var list = ObjectMapper.Map<IEnumerable<ModelType>, IReadOnlyList<ModelTypeListDto>>(entitys);
                return list;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 结构查询模型【带分页】
        /// </summary>
        /// <param name="modelName">模型id</param>
        /// <param name="glId">主键</param>
        /// <param name="pGlid">父id</param>
        /// <param name="externalId">构建id</param>
        /// <param name="top">多少条[默认全部]</param>
        /// <returns></returns>
        public async Task<PagedResultDto<ModelTypeListDto>> GetTypeChildId(string modelName, string glId, string pGlid, string externalId, int top = -1)
        {
            GetListModelTypeInput input = new GetListModelTypeInput();
            input.ModelName = modelName;
            input.Glid = glId;
            input.ExternalId = externalId;
            input.PGlid = pGlid;
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query
                .WhereIf(!input.ModelName.IsNullOrEmpty(), o => o.ModelName == input.ModelName)
                .WhereIf(!input.Glid.IsNullOrEmpty(), o => o.Glid == input.Glid)
                .WhereIf(!input.PGlid.IsNullOrEmpty(), o => o.PGlid == input.PGlid)
                .WhereIf(!input.ExternalId.IsNullOrEmpty(), o => o.ExternalId == input.ExternalId);

            if (top >= 0)
            {
                Query.Take(top);
            }
            var totalCount = await AsyncQueryableExecuter.CountAsync(Query);
            Query = ApplySorting(Query, input);
            Query = ApplyPaging(Query, input);
            var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
            var list = ObjectMapper.Map<IEnumerable<ModelType>, IReadOnlyList<ModelTypeListDto>>(entitys);
            return new PagedResultDto<ModelTypeListDto>(totalCount, list);
        }

        /// <summary>
        /// 根据父id获取组件id
        /// </summary>
        /// <param name="modelName">模型名称</param>
        /// <param name="groupName">模型组名称</param>
        /// <param name="glId">主键</param>
        /// <param name="pGlid">父id</param>
        /// <param name="top">多少条[默认全部]</param>
        /// <returns></returns>
       // [HttpGet("/api/app/model-type/child-id-list")]
        public async Task<List<string>> GetTypeChildIdList(string modelName, string groupName, string glId, string pGlid, int top = -1)
        {
            List<string> arrayId = new();
            GetListModelTypeInput input = new GetListModelTypeInput();
            input.ModelName = modelName;
            input.GroupName = groupName;
            input.Glid = glId;
            input.PGlid = pGlid;
            input.ExternalId = "0";
            var Query = await base.CreateFilteredQueryAsync(input);
            Query = Query
                .WhereIf(!input.ModelName.IsNullOrEmpty(), o => o.ModelName == input.ModelName)
                .WhereIf(!input.GroupName.IsNullOrEmpty(), o => o.GroupName == input.GroupName)
                .WhereIf(!input.Glid.IsNullOrEmpty(), o => o.Glid == input.Glid)
                .WhereIf(!input.PGlid.IsNullOrEmpty(), o => o.PGlid == input.PGlid)
                .WhereIf(!input.ExternalId.IsNullOrEmpty(), o => o.ExternalId == input.ExternalId);

            var entitylist = await AsyncQueryableExecuter.ToListAsync(Query);
            if (top >= 0)
            {
                entitylist.Take(top);
            }
            //查找模型下全部组件id
            var AllCharList = await Repository.Where(o => o.ModelName == modelName).ToListAsync();
            List<string> CharListId = SelCharlistItema(AllCharList,arrayId, entitylist);//存储返回的值
            return CharListId;
        }

        /// <summary>
        /// 递归查询所有子数据的id
        /// </summary>
        /// <param name="all"></param>
        /// <param name="arrayId">空的子数据集合</param>
        /// <param name="modelType">要添加的数据</param>        
        /// <returns></returns>
        private List<string> SelCharlistItema(List<ModelType> all,List<string> arrayId, List<ModelType> modelType)
        {
            foreach (var item in modelType)
            {
                if (item.ExternalId != "0")
                {
                    arrayId.Add(item.ExternalId);
                }
                else
                {
                    var CharTypeList = all.Where(o => o.ModelName == item.ModelName && o.PGlid == item.Glid).ToList();
                    SelCharlistItema(all,arrayId, CharTypeList);
                }
            }
            return arrayId;
        }
    }
}