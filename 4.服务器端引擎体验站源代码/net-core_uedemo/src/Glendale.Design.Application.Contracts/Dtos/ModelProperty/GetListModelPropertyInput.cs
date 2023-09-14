using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelProperty
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public class GetListModelPropertyInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 轻量化名称
        /// </summary>
        public virtual string ModelName { get; set; }
        /// <summary>
        /// 构件id，对应结构表externalId
        /// </summary>
        public virtual string ExternalId { get; set; }
        public GetListModelPropertyInput()
        {
            Sorting = "";
        }
    }
}
