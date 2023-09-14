using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Message
{
    public class GetListMessageInput : PagedAndSortedResultRequestDto
    {
        public Guid? ProjectId {get; set; }
        /// <summary>
        /// 关键字
        /// </summary>  
        public string Keyword { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>        
        public int IsRead { get; set; } = -1;
        

        /// <summary>
        /// 
        /// </summary>       
        public string StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>       
        public string EndDate{ get; set; }

        public GetListMessageInput()
        {
            Sorting = "CreationTime desc";
        }
    }
}
