using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Glendale.Design.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 项目图片
    /// </summary>
    public class ProjectImage : CreationAuditedEntity<Guid>
    {        
        #region 属性
        /// <summary>
        /// 项目ID
        /// </summary>
        [Required]
        public virtual Guid ProjectId { get; set; }
        
        /// <summary>
        /// 图片
        /// </summary>
        [Required]
        public virtual string BlobName { get; set; }

        /// <summary>
        /// 是否封面
        /// </summary>
        public virtual bool IsCover { get; set; } 
         
        #endregion

        #region 导航属性
        [NotMapped]
        public virtual Project Project { get; set; }         
        #endregion
        
    }
}
