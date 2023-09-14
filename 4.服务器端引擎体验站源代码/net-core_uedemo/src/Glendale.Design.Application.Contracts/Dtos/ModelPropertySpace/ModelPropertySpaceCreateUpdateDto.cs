using System;
using System.ComponentModel.DataAnnotations;

namespace Glendale.Design.Dtos.ModelPropertySpace
{
    /// <summary>
    /// 模型楼层数据
    /// </summary>
    public class ModelPropertySpaceCreateUpdateDto
    {  /// <summary>
       /// 外键Id
       /// </summary>
        [MaxLength(256)]
        public virtual string Glid { get; set; }
        /// <summary>
        /// 构件id，对应结构表externalId
        /// </summary>
        public virtual string ExternalId { get; set; }
        /// <summary>
        /// 属性大值
        /// </summary>
        public virtual string PropertyTypeName { get; set; }

        /// <summary>
        /// 属性小值
        /// </summary>
        public virtual string PropertySetName { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public virtual string PropertyName { get; set; }

        /// <summary>
        /// 当前属性ifc文件路径
        /// </summary>
        public virtual string Ifcurl { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public virtual string Value { get; set; }
        public virtual string ModelName { get; set; }
        public virtual string GroupName { get; set; }
    }
}