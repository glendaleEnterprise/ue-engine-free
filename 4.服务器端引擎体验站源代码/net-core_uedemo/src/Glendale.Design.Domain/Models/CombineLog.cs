using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Glendale.Design.Models
{
    /// <summary>
     /// 合模日志
     /// </summary>
    public class CombineLog : CreationAuditedEntityWithUser<Guid, IdentityUser>
    {
        /// <summary>
        /// 合模Id
        /// </summary>
        public virtual Guid CombineId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Required, MaxLength(100)]
        public virtual string Remark { get; set; }

        #region 导航属性
        /// <summary>
        /// 合模
        /// </summary>
        [NotMapped]
        public virtual Combine Combine { get; set; }
        #endregion
    }
}
