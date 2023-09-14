using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ModelType
{
    public class ModelTypeViewDto : EntityDto<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 子数据
        /// </summary>
        public virtual ICollection<ModelTypeViewDto> Children { get; set; }
    }
}
