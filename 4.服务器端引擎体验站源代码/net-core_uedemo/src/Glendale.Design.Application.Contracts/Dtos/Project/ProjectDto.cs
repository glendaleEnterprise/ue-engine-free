using Glendale.Design.Dtos.OrganizationUnit;
using Glendale.Design.Dtos.ProjectImage;
using Glendale.Design.Dtos.ProjectUser;
using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace Glendale.Design.Dtos.Project
{
    /// <summary>
    /// 项目
    /// </summary>
    public class ProjectDto : FullAuditedEntityDto<Guid>
    {
         
        /// <summary>
        /// 项目名称
        /// </summary>
        public virtual string ProjectName { get; set; }
         
        public virtual string ShortName { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public virtual string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public virtual string City { get; set; }
        /// <summary>
        /// 区\县
        /// </summary>
        public virtual string District { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public virtual string Address { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }        
         
        /// <summary>
        /// 开始日期
        /// </summary>
        public virtual DateTime BeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public virtual DateTime? FinishDate { get; set; }
        /// <summary>
        /// 是否开放
        /// </summary>
        public virtual bool IsPublic { get; set; }
        
        public virtual string Tags { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public virtual Guid? OrgId { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        public virtual Guid? ManageId { get; set; }

        #region 补充属性

        /// <summary>
        /// 项目参与用户
        /// </summary>
        public virtual ICollection<ProjectUserListDto> ProjectUsers { get; set; }

         
        /// <summary>
        /// 封面图片（多个）
        /// </summary>        
        public virtual ICollection<ProjectImageDto> ProjectImages { get; set; }
        /// <summary>
        /// 创建人账号
        /// </summary>
        public virtual string CreationAccount { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public virtual string CreationName { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public virtual OrganizationUnitDto OrgInfo { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        public virtual IdentityUserDto ManageInfo { get; set; }
        #endregion
    }
}
