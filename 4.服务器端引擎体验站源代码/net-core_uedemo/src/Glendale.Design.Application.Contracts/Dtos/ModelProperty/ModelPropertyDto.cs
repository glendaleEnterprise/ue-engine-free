using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelProperty
{
	/// <summary>
	/// 模型楼层数据
	/// </summary>
	public class ModelPropertyDto:EntityDto<int>
	{
        /// <summary>
        /// 外键Id
        /// </summary>
        [MaxLength(256)]
        public virtual string Glid { get; set; }
        /// <summary>
        /// 构件id，对应结构表externalId
        /// </summary>
        [MaxLength(256)]
        public virtual string ExternalId { get; set; }
        /// <summary>
        /// 属性大值
        /// </summary>
        [MaxLength(2500)]
        public virtual string PropertyTypeName { get; set; }

        /// <summary>
        /// 属性小值
        /// </summary>
        [MaxLength(2500)]
        public virtual string PropertySetName { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        [MaxLength(250)]
        public virtual string PropertyName { get; set; }

        /// <summary>
        /// 当前属性ifc文件路径
        /// </summary>
        public virtual string Ifcurl { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public virtual string Value { get; set; }

        [MaxLength(1000)]
        public virtual string ModelName { get; set; }

        //[MaxLength(1000)]
        //public virtual string GroupName { get; set; }
    }
}
