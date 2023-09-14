using Glendale.Design.Dtos.ModelTree;
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
    /// 模型楼层表
    /// </summary>  
    [AllowAnonymous]
    public class ModelTreeAppService : CrudAppService<ModelTree, ModelTreeDto, ModelTreeListDto, int, GetListModelTreeInput,
                            ModelTreeCreateUpdateDto, ModelTreeCreateUpdateDto>, IModelTreeAppService
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }      
        public ModelTreeAppService(IRepository<ModelTree, int> repository)
            : base(repository)
        {
            
        }

        [RemoteService(false)]
        public override Task<ModelTreeDto> CreateAsync(ModelTreeCreateUpdateDto input)
        {
            return base.CreateAsync(input);
        }

        [RemoteService(false)]
        public override Task<ModelTreeDto> UpdateAsync(int id, ModelTreeCreateUpdateDto input)
        {
            return base.UpdateAsync(id, input);
        }

        [RemoteService(false)]
        public override Task<PagedResultDto<ModelTreeListDto>> GetListAsync(GetListModelTreeInput input)
        {
            return base.GetListAsync(input);
        }

        [RemoteService(false)]
        public override Task DeleteAsync(int id)
        {
            return base.DeleteAsync(id);
        }

        [RemoteService(false)]
        public override Task<ModelTreeDto> GetAsync(int id)
        {
            return base.GetAsync(id);
        }

        /// <summary>
        /// 根据条件查询，返回List
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<ModelTreeListDto>> GetAllListAsync(GetListModelTreeInput input)
        {
            try
            {
                var Query = await base.CreateFilteredQueryAsync(new GetListModelTreeInput());
                Query = Query.Where(o => o.ModelName == input.ModelName)
                    .WhereIf(!input.GroupName.IsNullOrEmpty(), o => o.GroupName == input.GroupName)
                    .WhereIf(!input.Glid.IsNullOrEmpty(), o => o.Glid == input.Glid)
                    .WhereIf(!input.PGlid.IsNullOrEmpty(), o => o.PGlid == input.PGlid)
                    .WhereIf(input.Level != null, o => o.Level == input.Level)
                    .WhereIf(!input.Name.IsNullOrEmpty(), o => o.Name == input.Name)
                    .WhereIf(!input.ExternalId.IsNullOrEmpty(), o => o.ExternalId == input.ExternalId);
                var entitys = await AsyncQueryableExecuter.ToListAsync(Query);
                var list = ObjectMapper.Map<IEnumerable<ModelTree>, IReadOnlyList<ModelTreeListDto>>(entitys);
                return list;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取第一层数据
        /// </summary>
        /// <param name="modelName">模型名称</param>
        /// <returns></returns>
        //[HttpGet("/api/app/model-tree/firsttree")]
        //public async Task<List<ModelTreeListDto>> GetFirstTreeListAsync(string modelName)
        //{
        //    var entitys = await Repository.Where(o => o.ModelName == modelName && o.Level == 0).ToListAsync();
        //    return await MapToGetListOutputDtosAsync(entitys);
        //}

        /// <summary>
        /// 获取模型楼层所有子id
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="glId">模型id</param>
        /// <returns></returns>
        //[HttpGet("/api/app/model-tree/child-id-list")]
        public async Task<List<string>> GetTreeChildId(string modelName, string glId)
        {
            var db = await Repository.GetDbSetAsync();
            var list = await db.FromSqlRaw(string.Format(@"with recursive tp1 as(
                select glid,pGlid,externalId from model_tree_{0} where glid = '{1}'
                union all
                select tb2.glid,tb2.pGlid,tb2.externalId from model_tree_{0} tb2, tp1 WHERE tb2.pGlid = tp1.glid
            )select * from tp1 where externalId <> '0'", modelName, glId)).ToListAsync();

            return list.Select(s => s.Glid).ToList();

            ////查找模型下全部组件id
            //List<string> arrayId = new();
            //var AllCharList = await Repository.Where(o => o.ModelName == modelName).ToListAsync();
            //var entity = AllCharList.Where(o => glId == o.Glid).FirstOrDefault();
            //if (entity == null) 
            //{ 
            //    return arrayId; 
            //};
            //List<ModelTree> entityList = new();
            //entityList.Add(entity);
            //List<string> CharListId = SelCharlistItema(AllCharList, arrayId, entityList);//存储返回的值
            //return CharListId;
        }

        private List<string> SelCharlistItema(List<ModelTree> all, List<string> arrayId, List<ModelTree> modelTree)
        {
            arrayId.AddRange(modelTree.Where(s => s.ExternalId != "0").Select(s => s.ExternalId));

            var glids = modelTree.Select(s => s.Glid);
            var CharTypeList = all.Where(o => glids.Contains(o.PGlid)).ToList();

            if (CharTypeList.Any())
            {
                SelCharlistItema(all, arrayId, CharTypeList);
            }

            return arrayId;
        }
        /// <summary>
        /// 通过轻量化名称和glid，获取模型结构树
        /// </summary>
        /// <param name="pid">glid</param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public async Task<List<ModelTreeListDto>> GetTreeAsync(string modelName, string pid)
        {
            var Query = Repository.Where(p => p.ModelName == modelName);
            var list = await AsyncQueryableExecuter.ToListAsync(Query);
            var first = list.FirstOrDefault(p => p.Glid == pid);
            var result = GetTree(list, pid, first);
            return ObjectMapper.Map<IEnumerable<ModelTree>, List<ModelTreeListDto>>(result);
        }

        /// <summary>
        /// 通过轻量化名称和GLID，获取所有模型结构树
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public async Task<List<ModelTreeListDto>> GetChildrenTreeAsync(string modelName, string pid)
        {
            var Query = Repository.Where(p => p.ModelName == modelName);
            var list = await AsyncQueryableExecuter.ToListAsync(Query);
            var first = list.FirstOrDefault(p => p.Glid == pid);
            var result = GetTree(list, pid, first);
            return ObjectMapper.Map<IEnumerable<ModelTree>, List<ModelTreeListDto>>(result);
        }

        /// <summary>
        /// 获取模型结构
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="PGlid"></param>
        /// <param name="modelTree"></param>
        /// <returns></returns>
        private List<ModelTree> GetTree(List<ModelTree> entitys, string PGlid, ModelTree modelTree)
        {
            List<ModelTree> list = new List<ModelTree>();

            var modelList = entitys.Where(x => x.PGlid == PGlid);

            if (modelList != null && modelList.Count() > 0)
            {
                foreach (var item in modelList)
                {
                    list.AddRange(GetTree(entitys, item.Glid, item));
                }
            }
            else
            {
                list.Add(modelTree);
            }
            return list;
        }

        /// <summary>
        /// 获取部分模型结构树(仅包括自身和其子级)
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<ModelTreeListDto>> GetPartTree(string modelName, string[] pids)
        {
            try
            {
                List<ModelTree> result = null;
                var Query = Repository.Where(p => p.ModelName == modelName && (pids.Contains(p.PGlid) || pids.Contains(p.Glid)));
                var list = await AsyncQueryableExecuter.ToListAsync(Query);
                result = list.Where(p => p.PGlid == "0").ToList();
                foreach (var item in result)
                {
                    item.Children = list.Where(p => p.PGlid == item.Glid).ToList();
                }
                return ObjectMapper.Map<IEnumerable<ModelTree>, List<ModelTreeListDto>>(result);
            }
            catch
            {
                return null;
            }
        }
    }

}
