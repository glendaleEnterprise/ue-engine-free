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
    /// 目录用户
    /// </summary>
    public class ProjectFolderUser : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public virtual Guid ProjectId { get; set; }
        public virtual Guid ProjectFolderId { get; set; }
        public virtual Guid UserId { get; set; }
        public virtual string UserRoleNames { get; set; }

        #region 导航属性
        public virtual ProjectFolder ProjectFolder { get; set; }
        public virtual IdentityUser User { get; set; }
        #endregion
    }
}
