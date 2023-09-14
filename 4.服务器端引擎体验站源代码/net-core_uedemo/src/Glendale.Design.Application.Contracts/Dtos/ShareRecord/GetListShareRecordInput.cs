using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ShareRecord
{
    public class GetListShareRecordInput : PagedAndSortedResultRequestDto
    {
         
        /// <summary>
        /// 关键字
        /// </summary>
        public virtual string Keyword { get; set; }
         
        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        public GetListShareRecordInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}
