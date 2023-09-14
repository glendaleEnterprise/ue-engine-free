using System;
using Volo.Abp.Domain.Entities;

namespace Glendale.Design.Dtos.RoleOrgJoin
{
    public class RoleOrgJoinCreateUpdateDto
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        public virtual Guid RoleId { get; set; }
        /// <summary>
        /// 组织机构
        /// </summary>
        public virtual Guid OrgId { get; set; }
    }
}
