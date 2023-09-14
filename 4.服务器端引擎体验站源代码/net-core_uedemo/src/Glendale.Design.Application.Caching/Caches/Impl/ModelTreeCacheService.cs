using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glendale.Design.Core.Extensions;
using Glendale.Design.Dtos.ModelTree;

namespace Glendale.Design.Caches
{
    public class ModelTreeCacheService : CachingServiceBase, IModelTreeCacheService
    {
        private const string BIM_Prefix = CachePrefix.BIM;
        private const string KeyBIMModelTree = BIM_Prefix + "modeltree.{0}";

        /// <summary>
        /// 获取基础数据列表
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public async Task<List<ModelTreeListDto>> GetModelTreeByParentAsync(string parent, Func<Task<List<ModelTreeListDto>>> factory)
        {
            return await Cache.GetOrAddAsync(KeyBIMModelTree.FormatWith(parent), factory, CacheStrategy.NEVER);
        }

        public async Task RemoveAsync(string parent)
        {
            await this.Cache.RemoveAsync(KeyBIMModelTree.FormatWith(parent));
        }
    }
}