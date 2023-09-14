using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.DocumentLog
{
    public class DocumentLogDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 文档Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public virtual string CreatorName { get; set; }
    }
}
