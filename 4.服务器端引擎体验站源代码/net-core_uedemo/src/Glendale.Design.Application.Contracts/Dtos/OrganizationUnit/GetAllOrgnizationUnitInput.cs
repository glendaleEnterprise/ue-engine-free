using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.OrganizationUnit
{
    public class GetAllOrgnizationUnitInput : ISortedResultRequest
    {
        public GetAllOrgnizationUnitInput()
        {
            Sorting = "Code";
        }
        public string Filter { get; set; }
        public string Sorting { get; set; }
    }
}
