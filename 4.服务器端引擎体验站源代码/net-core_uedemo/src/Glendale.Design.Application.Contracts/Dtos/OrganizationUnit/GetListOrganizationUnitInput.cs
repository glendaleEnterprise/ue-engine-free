using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.OrganizationUnit
{
    public class GetListOrganizationUnitInput : PagedAndSortedResultRequestDto
    {
        public Guid? ParentId { get; set; }
        public string Filter { get; set; }
        public GetListOrganizationUnitInput()
        {
            Sorting = "Code Asc";
        }
    }
}
