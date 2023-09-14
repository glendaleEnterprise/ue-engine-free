using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ProjectFolderUser
{
    public class ProjectFolderUserDto : EntityDto<Guid>
    {
        public Guid ProjectId { get; set; }
        public Guid ProjectFolderId { get; set; }
        public virtual string UserRoleNames { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string PhoneNumber { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public virtual List<string> OrgNames { get; set; }
    }
}
