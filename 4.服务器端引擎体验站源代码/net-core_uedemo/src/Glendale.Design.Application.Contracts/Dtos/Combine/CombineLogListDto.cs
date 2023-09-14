using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.Combine
{
    public class CombineLogListDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 合模Id
        /// </summary>
        public virtual Guid CombineId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }

        #region 补充属性
        /// <summary>
        /// 创建人名字
        /// </summary>
        public virtual string CreationName { get; set; }
        #endregion
    }
}
