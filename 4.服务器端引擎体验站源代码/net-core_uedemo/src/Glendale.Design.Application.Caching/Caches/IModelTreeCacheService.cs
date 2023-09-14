using Glendale.Design.Dtos.ModelTree;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glendale.Design.Caches
{
    public interface IModelTreeCacheService
    {
        /// <summary>
        /// 获取基础数据列表
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<List<ModelTreeListDto>> GetModelTreeByParentAsync( string parent, Func<Task<List<ModelTreeListDto>>> factory);
        Task RemoveAsync(string parent);
    }
}