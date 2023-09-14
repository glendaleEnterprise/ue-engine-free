using System;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Dictionary
{
    /// <summary>
    /// 基础数据
    /// </summary>
    public class GetListDictionaryInput : PagedAndSortedResultRequestDto
    {
        
        

        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string Keyword { get; set; }
         
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool? Enable { get; set; }

        public GetListDictionaryInput()
        {
            Sorting = "Enable Desc,OrderIndex,CreationTime";
        }
    }
}
