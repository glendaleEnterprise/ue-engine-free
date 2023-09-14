using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glendale.Design.Core.Extensions;
using Glendale.Design.Dtos.ModelType;

namespace Glendale.Design.Caches
{
    public class DocumentVerCacheService : CachingServiceBase, IDocumentVerCacheService
    {
        private const string BIM_Prefix = CachePrefix.BIM;
        private const string KeyBIMDocumentVer = BIM_Prefix + "DocumentVer.{0}";
        private const string KeyBIMDocumentVerModelNames = BIM_Prefix + "DocumentVer.ModelNames";

        /// <summary>
        /// 获取模型名称列表
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public async Task<List<string>> GetModelNameListAsync(Func<Task<List<string>>> factory)
        {
            return await Cache.GetOrAddAsync(KeyBIMDocumentVerModelNames, factory, CacheStrategy.NEVER);
        }

        public async Task RemoveAsync()
        {
            await this.Cache.RemoveAsync(KeyBIMDocumentVerModelNames);
        }
    }
}