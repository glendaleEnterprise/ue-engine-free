using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Glendale.Design.Core;
using System.Linq;
using Glendale.Design.Enums;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 自定义漫游设置
    /// </summary>
	public class Roaming : FullAuditedEntity<Guid>
    {
        #region 属性

        /// <summary>
        /// 场景类型
        /// </summary>
        public virtual SceneTypeEnum SceneType { get; set; }


        /// <summary>
        /// 漫游类型
        /// </summary>
        public virtual RoamingTypeEnum RoamingType { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        [MaxLength(100)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 模型Id
        /// </summary>
        [MaxLength(50)]
        public virtual string ModelId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public virtual Guid? ProjectId { get; set; }

        /// <summary>
        /// 方案时长
        /// </summary>
        public virtual decimal? Time { get; set; }

        /// <summary>
        /// 方案备注
        /// </summary>
        [MaxLength(200)]
        public virtual string Remark { get; set; }
         
        /// <summary>
        /// 漫游记录
        /// </summary>
        [NotMapped]
        public virtual string Record  => $"[{this.RoamingPoints?.ToList().OrderBy(x => x.Sort).Select(x => x.Record).JoinAsString(",")}]";
        
        /// <summary>
        /// 子数据
        /// </summary>
        [NotMapped]
        public virtual ICollection<RoamingPoint> RoamingPoints { get; set; }
        #endregion
        
        
    }
}
