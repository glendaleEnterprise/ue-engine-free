using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentLog
{
    public class GetListDocumentLogInput : PagedAndSortedResultRequestDto
    {
        public virtual Guid DocumentId { get; set; }
        public virtual string Keyword { get; set; }

        public GetListDocumentLogInput()
        {
            Sorting = "CreationTime Desc";
        }
    }
}
