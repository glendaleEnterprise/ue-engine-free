using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentConfig
{
    public class DocumentConfigInput : PagedAndSortedResultRequestDto
    {
        public virtual Guid? DocumentVerId { get; set; }
        public virtual Guid? DocumentId { get; set; }
        public virtual string ModelName { get; set; }
        public virtual double? VersionNo { get; set; }

        public DocumentConfigInput()
        {
            Sorting = "CreationTime Desc";
        }
    }
}
