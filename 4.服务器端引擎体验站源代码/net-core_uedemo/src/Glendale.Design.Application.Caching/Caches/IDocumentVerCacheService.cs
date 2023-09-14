using Glendale.Design.Dtos.ModelType;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glendale.Design.Caches
{
    public interface IDocumentVerCacheService
    {
        /// <summary>
        /// 获取模型名称列表
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<List<string>> GetModelNameListAsync(Func<Task<List<string>>> factory);
        Task RemoveAsync();
    }
}