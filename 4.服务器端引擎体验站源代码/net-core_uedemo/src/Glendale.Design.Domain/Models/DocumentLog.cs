using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 文档日志
    /// </summary>
    public class DocumentLog : CreationAuditedEntityWithUser<Guid, IdentityUser>
    {
        /// <summary>
        /// 文档Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Required, MaxLength(100)]
        public virtual string Remark { get; set; }

        #region 导航属性
        /// <summary>
        /// 文档
        /// </summary>
        [NotMapped]
        public virtual Document Document { get; set; }
        #endregion

        public DocumentLog() { }

        public DocumentLog(Guid documentId, string remark) 
        {
            DocumentId = documentId;
            Remark = remark;           
        }
    }
}
