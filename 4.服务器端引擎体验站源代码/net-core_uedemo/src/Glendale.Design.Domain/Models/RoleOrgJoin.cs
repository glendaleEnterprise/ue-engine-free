using Glendale.Design.Enums;
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
    /// 角色和组织机构关系表
    /// </summary>
    public class RoleOrgJoin : FullAuditedEntityWithUser<Guid, IdentityUser>
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        public virtual Guid RoleId { get; set; }
        /// <summary>
        /// 组织机构
        /// </summary>
        public virtual Guid OrgId{ get; set; }
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public virtual IdentityRole RoleInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual OrganizationUnit OrgInfo { get; set; }
        #endregion

    }
}
