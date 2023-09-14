using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Combine
{
    public class GetListCombineFlattenInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 合模Id
        /// </summary>
        public virtual Guid CombineId { get; set; }

        /// <summary>
        /// 类型 0：压平； 1：裁剪；
        /// </summary>
        public virtual int? FlattenType { get; set; }

        /// <summary>
        /// 压平名称
        /// </summary>
        public virtual string FlattenName { get; set; }
        public GetListCombineFlattenInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}
