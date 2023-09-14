using Glendale.Design.Dtos.ProjectFolderUser;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos.ProjectFolder
{
    public class ProjectFolderCreateDto
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        [Required]
        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        public virtual Guid? ParentId { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>
        [Required, MaxLength(100)]
        public virtual string FolderName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual List<ProjectFolderCreateDto> Children { get; set; }

        /// <summary>
        /// 目录人员
        /// </summary>
        public virtual ICollection<ProjectFolderUserCreateDto> ProjectFolderUsers { get; set; }
    }
}
