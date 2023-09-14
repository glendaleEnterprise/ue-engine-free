using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentVer
{
    public class GetListDocumentVersionInput : PagedAndSortedResultRequestDto
    {
        public virtual Guid DocumentId { get; set; }

        public virtual string Keyword { get; set; }
        ///// <summary>
        ///// 轻量化状态
        ///// </summary>
        //public virtual DocStatusEnum? Status { get; set; }        

        public GetListDocumentVersionInput()
        {
            Sorting = "CreationTime Desc";
        }
    }
}
