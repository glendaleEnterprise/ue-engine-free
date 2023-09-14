using System;
using System.Collections.Generic;
using System.Text;
using Glendale.Design.Enums;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ProjectUser
{
    /// <summary>
    /// 项目用户
    /// </summary>
    public class ProjectUserDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public virtual Guid ProjectId { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public virtual Guid UserId { get; set; }

        public virtual bool IsManager { get; set; }

    }
}
