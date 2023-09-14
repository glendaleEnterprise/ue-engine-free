using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ProjectFolderUser
{
    public class PermissionGrantProjectUserDto : EntityDto<Guid>
    {
        //public virtual Guid? TenantId { get; set; }
        public virtual string Name { get; set; }
        public virtual string ProviderName { get; set; }
        public virtual string ProviderKey { get; set; }

        public virtual List<PermissionGrantProjectUserDto> Children { get; set; }
    }
}
