using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;

namespace Glendale.Design.Dtos.OrganizationUnit
{
    public class OrganizationUnitCreateDto : OrganizationUnitCreateOrUpdateDtoBase
    {
        public Guid? ParentId { get; set; }
        public string Code { get; set; }

    }
}
