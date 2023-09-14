using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
    /// 文档用户权限
    /// </summary>
    public class DocumentPermission : CreationAuditedEntity<Guid>
    {
        public virtual Guid UserId { get; set; }

        public virtual Guid DocumentId { get; set; }

        #region 导航属性
        [NotMapped]
        public virtual Document Document { get; set; }
        [NotMapped]
        public virtual IdentityUser User { get; set; }

        #endregion
    }
}
