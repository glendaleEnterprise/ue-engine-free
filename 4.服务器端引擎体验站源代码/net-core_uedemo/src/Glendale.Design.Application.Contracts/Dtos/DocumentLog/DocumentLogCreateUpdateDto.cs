using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.DocumentLog
{
    public class DocumentLogCreateUpdateDto
    {
        /// <summary>
        /// 文档Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
    }
}
