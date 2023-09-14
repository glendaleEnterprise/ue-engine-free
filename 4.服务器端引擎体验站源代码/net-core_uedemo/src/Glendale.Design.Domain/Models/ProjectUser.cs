using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Glendale.Design.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 项目参与人员
    /// </summary>
    public class ProjectUser : CreationAuditedEntity<Guid>
    {        
        #region 属性
        /// <summary>
        /// 项目Id
        /// </summary>
        [Required]
        public virtual Guid ProjectId { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Required]
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// 是否项目负责人
        /// </summary>
        public virtual bool IsManager { get; set; } 
         
        #endregion

        #region 导航属性
        [NotMapped]
        public virtual Project Project { get; set; }
        [NotMapped]
        public virtual IdentityUser User { get; set; }
        #endregion
        
    }
}
