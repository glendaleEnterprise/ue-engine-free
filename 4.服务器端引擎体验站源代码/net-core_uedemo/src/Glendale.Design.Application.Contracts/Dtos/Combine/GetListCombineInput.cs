using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Combine
{
    public class GetListCombineInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 所属目录Id
        /// </summary>
        public virtual Guid FolderId { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Guid? ProjectId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(50)]
        public virtual string CombineName { get; set; }
        public GetListCombineInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}
