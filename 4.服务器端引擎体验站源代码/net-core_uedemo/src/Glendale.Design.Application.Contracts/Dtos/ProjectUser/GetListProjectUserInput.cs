using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ProjectUser
{
    /// <summary>
    /// 项目用户
    /// </summary>
    public class GetListProjectUserInput : PagedAndSortedResultRequestDto
    {
        public virtual Guid ProjectId { get; set; }
         
        public GetListProjectUserInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}
