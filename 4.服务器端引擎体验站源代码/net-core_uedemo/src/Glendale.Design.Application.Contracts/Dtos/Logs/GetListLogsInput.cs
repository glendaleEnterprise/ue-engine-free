using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos
{
    public class GetListLogsInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 关键字
        /// </summary>  
        public string Keyword { get; set; }
        /// <summary>
        /// 
        /// </summary>       
        public int DateType { get; set; }
        /// <summary>
        /// 
        /// </summary>       
        public Guid? UserId { get; set; }
        public GetListLogsInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}
