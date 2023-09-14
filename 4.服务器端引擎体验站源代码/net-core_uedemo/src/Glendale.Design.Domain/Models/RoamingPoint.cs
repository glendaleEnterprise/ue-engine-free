using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 自定义漫游点设置
    /// </summary>
	public class RoamingPoint : Entity<Guid>
    {
        #region 属性
        /// <summary>
        /// 漫游ID
        /// </summary>
        public virtual Guid? RoamingId { get; set; }

        /// <summary>
        /// 漫游记录
        /// </summary>
        [MaxLength(200)]
        public virtual string Record { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public virtual int Sort { get; set; }
        #endregion
        #region 导航属性
        /// <summary>
        /// 漫游
        /// </summary>
        [NotMapped]
        public virtual Roaming Roaming { get; set; }

        #endregion
        protected RoamingPoint()
        {
        }
        public RoamingPoint(Guid id, Guid? roamingId, string record, int sort)
            : base(id)
        {
            RoamingId = roamingId;
            Record = record;
            Sort = sort;
        }
    }
}
