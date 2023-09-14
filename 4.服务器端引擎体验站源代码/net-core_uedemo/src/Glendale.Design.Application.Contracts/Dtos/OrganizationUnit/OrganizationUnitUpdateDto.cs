using System;
using Volo.Abp.Domain.Entities;

namespace Glendale.Design.Dtos.OrganizationUnit
{
    public class OrganizationUnitUpdateDto : OrganizationUnitCreateOrUpdateDtoBase, IHasConcurrencyStamp
    {
        public Guid? ParentId { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}
