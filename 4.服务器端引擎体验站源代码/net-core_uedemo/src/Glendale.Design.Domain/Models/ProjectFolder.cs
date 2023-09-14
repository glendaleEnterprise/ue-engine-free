using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Glendale.Design.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 项目目录
    /// </summary>
    public class ProjectFolder : AuditedEntity<Guid>
    {        
        #region 属性
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
        [Required,MaxLength(100)]        
        public virtual string FolderName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public virtual string Remark { get; set; }

        #endregion



        #region 导航属性
        /// <summary>
        /// 父级
        /// </summary>
        [NotMapped]
        public virtual ProjectFolder Parent { get; set; }

        /// <summary>
        /// 子数据
        /// </summary>
        [NotMapped]
        public virtual ICollection<ProjectFolder> Children { get; set; }

        [NotMapped]
        public virtual Project Project { get; set; }

        /// <summary>
        /// 目录用户
        /// </summary>
        [NotMapped]
        public virtual ICollection<ProjectFolderUser> ProjectFolderUsers { get; set; }
        
        [NotMapped]
        public virtual string UserStatus { get; set; }
        #endregion
        public ProjectFolder()
        {
        }
        public ProjectFolder(Guid id)
            : base(id)
        {
        }

    }
}
