using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelProperty
{
    public class ModelPropertyViewDto : EntityDto<int>
    {
        /// <summary>
        /// 外键Id
        /// </summary>
        public virtual string Glid { get; set; }
        /// <summary>
        /// 构件id，对应结构表externalId
        /// </summary>
        public virtual string ExternalId { get; set; }

        /// <summary>
        /// 属性小值
        /// </summary>
        public virtual string PropertySetName { get; set; }
        public virtual string PropertyName { get; set; }
        public virtual string ModelName { get; set; }
    }
}
