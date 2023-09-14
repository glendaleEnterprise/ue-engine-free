using Glendale.Design.Dtos.ProjectFolderUser;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ProjectFolder
{
    public class ProjectFolderDto:AuditedEntityDto<Guid>
    {
        /// <summary>
        /// 项目ID
        /// </summary>       
        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        public virtual Guid? ParentId { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>       
        public virtual string FolderName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        public virtual string Remark { get; set; }

        public virtual ICollection<ProjectFolderUserListDto> ProjectFolderUsers { get; set; }
    }
}
