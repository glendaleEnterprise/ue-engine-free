using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Localization;
using Volo.Abp.Application.Services;

namespace Glendale.Design
{
    /* Inherit your application services from this class.
     */
    public abstract class DesignAppService : ApplicationService
    {
        protected DesignAppService()
        {
            LocalizationResource = typeof(DesignResource);
        }
    }
    public static class Extend
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
