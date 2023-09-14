using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Document
{
    public class GetListDocumentInput : PagedAndSortedResultRequestDto
    {
        public virtual Guid ProjectFolderId { get; set; }         
        public virtual DocStatusEnum? Status { get; set; }
        
        public virtual string Keyword { get; set; }
        /// <summary>
        /// 文档类型
        /// </summary>
        public virtual DocTypeEnum?[] DocumentType { get; set; }
        public GetListDocumentInput()
        {
            Sorting = "CreationTime Desc";
        }
    }
}
