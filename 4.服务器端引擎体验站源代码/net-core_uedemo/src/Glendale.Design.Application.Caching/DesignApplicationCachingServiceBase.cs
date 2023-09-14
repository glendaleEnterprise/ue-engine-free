using Glendale.Design.Core.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Glendale.Design
{
    public class CachingServiceBase : ITransientDependency
    {
        public IDistributedCache Cache { get; set; }

        public async Task RemoveAsync(string key, long cursor = 0, long? count = 10)
        {
            var keys = new List<string>();
            string prefix = HandlePrefix(key);
            var scan = await RedisHelper.ScanAsync(cursor, pattern: prefix, count: count);
            if (scan.Items.Length > 0) keys.AddRange(scan.Items);
            while (scan.Cursor > 0)
            {
                scan = await RedisHelper.ScanAsync(scan.Cursor, pattern: prefix, count: count);
                if (scan.Items.Length > 0) keys.AddRange(scan.Items);
            }

            if (keys.Any() && key.IsNotNullOrEmpty())
            {
                await RedisHelper.DelAsync(keys.Where(x => x.StartsWith(key)).ToArray());
            }
        }
        /// <summary>
        /// Handles the prefix of CacheKey.
        /// </summary>
        /// <param name="prefix">Prefix of CacheKey.</param>
        /// <exception cref="ArgumentException"></exception>
        private static string HandlePrefix(string prefix)
        {
            // Forbid
            if (prefix.Equals("*"))
                throw new ArgumentException("the prefix should not equal to *");

            // Don't start with *
            prefix = new System.Text.RegularExpressions.Regex("^\\*+").Replace(prefix, "");

            // End with *
            if (!prefix.EndsWith("*", StringComparison.OrdinalIgnoreCase))
                prefix = string.Concat(prefix, "*");

            return prefix;
        }
    }

    public interface ICacheRemoveService
    {
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        Task RemoveAsync(string key, long cursor = 0, long? count = 10);
    }
}