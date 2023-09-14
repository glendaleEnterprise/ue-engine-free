using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.RoleOrgJoin
{
    public class GetListRoleOrgJoinInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        public virtual Guid? RoleId { get; set; }
        /// <summary>
        /// 组织机构
        /// </summary>
        public virtual Guid? OrgId { get; set; }
        public GetListRoleOrgJoinInput()
        {
            Sorting = "CreationTime Desc";
        }
    }
}
