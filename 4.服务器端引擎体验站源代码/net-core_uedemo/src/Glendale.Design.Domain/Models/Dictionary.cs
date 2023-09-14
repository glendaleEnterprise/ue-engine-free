using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 基础数据
    /// </summary>
	public class Dictionary : FullAuditedEntity<Guid>
    {
        #region 属性
        /// <summary>
        /// 父Id
        /// </summary>6
        public virtual Guid? ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(20)]
        public virtual string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [MaxLength(20)]
        public virtual string Value { get; set; }
        /// <summary>
        /// 排序值
        /// </summary>
        public virtual int OrderIndex { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool Enable { get; set; }
        /// <summary>
        /// 是否系统内置
        /// </summary>
        public virtual bool System { get; set; }
        #endregion
        #region 导航属性
        /// <summary>
        /// 父数据
        /// </summary>
        [NotMapped]
        public virtual Dictionary Parent { get; set; }
        /// <summary>
        /// 子数据
        /// </summary>
        [NotMapped]
        public virtual ICollection<Dictionary> Children { get; set; }
        #endregion
        protected Dictionary()
        {
        }
        public Dictionary(Guid id, Guid? parentId, string name, string value, int orderIndex, bool enable, bool system, ICollection<Dictionary> children)
            : base(id)
        {
            ParentId = parentId;
            Name = name;
            Value = value;
            OrderIndex = orderIndex;
            Enable = enable;
            System = system;
            Children = children;
        }
        public Dictionary(Guid id, Guid? parentId, string name, string value, int orderIndex, bool enable, bool system, Dictionary parent, ICollection<Dictionary> children)
            : base(id)
        {
            ParentId = parentId;
            Name = name;
            Value = value;
            OrderIndex = orderIndex;
            Enable = enable;
            System = system;
            Parent = parent;
            Children = children;
        }
    }
}
