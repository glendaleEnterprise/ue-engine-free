using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Glendale.Design.Dtos.ProjectUser
{
    /// <summary>
    /// 项目用户
    /// </summary>
    public class ProjectUserListDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public virtual Guid ProjectId { get; set; }       
        /// <summary>
        /// 用户
        /// </summary>
        public virtual Guid UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string UserName { get; set; }

        public virtual bool IsManager { get; set; }

        #region 补充属性

        /// <summary>
        /// 姓名
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string PhoneNumber { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public virtual List<string> OrgNames { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public virtual string Position { get; set; }
       
        #endregion

    }
}
