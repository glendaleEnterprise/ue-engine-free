using Glendale.Design.Dtos.OrganizationUnit;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Glendale.Design.Dtos.RoleOrgJoin
{
    public class RoleOrgJoinDto : FullAuditedEntityDto<Guid>
    {
        #region 属性
        /// <summary>
        /// 角色标识
        /// </summary>
        public virtual Guid RoleId { get; set; }
        /// <summary>
        /// 组织机构
        /// </summary>
        public virtual Guid OrgId { get; set; }
        #endregion 属性
        /// <summary>
        /// 
        /// </summary>
        public virtual IdentityRoleDto RoleInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual OrganizationUnitDto OrgInfo { get; set; }
    }
}
