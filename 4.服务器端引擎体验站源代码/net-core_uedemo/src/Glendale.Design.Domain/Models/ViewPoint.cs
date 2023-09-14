using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 视点设置
    /// </summary>
	public class ViewPoint : FullAuditedEntity<Guid>
    {
        #region 属性        

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid ProjectId { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(100)]
        public virtual string ViewName { get; set; }

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>         
        public virtual string BlobName { get; set; }

        
        [MaxLength(500)]
        public virtual string Position { get; set; }         

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public virtual int? Status { get; set; }

        #endregion
    }
}