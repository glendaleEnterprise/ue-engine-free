using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Project
{
    public class GetListProjectInput : PagedAndSortedResultRequestDto
    {            
        public virtual string Keyword { get; set; }

        public virtual int? Year { get; set; }

        public virtual bool? IsPublic { get; set; }

        public virtual string Tag { get; set; }


        public GetListProjectInput()
        {
            Sorting = "CreationTime Desc";
        }
    }
}
