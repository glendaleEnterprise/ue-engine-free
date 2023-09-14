using System;
using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.Dtos.Dictionary
{
    /// <summary>
    /// 基础数据
    /// </summary>
    public class DictionaryCreateUpdateDto
    {
        /// <summary>
        /// 父名称
        /// </summary>
        public virtual string Parent { get; set; }
        /// <summary>
        /// 父名称Id
        /// </summary>
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
        /// 是否启用
        /// </summary>
        public virtual bool Enable { get; set; }

        public virtual int OrderIndex { get; set; }
    }
}