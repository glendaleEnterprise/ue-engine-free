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
    public class ProjectListDto : CreationAuditedEntityDto<Guid>
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


        public virtual string Address { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }

        public virtual DateTime BeginDate { get; set; }

        public virtual DateTime? FinishDate { get; set; }

        /// <summary>
        /// 是否公开
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
        /// 封面图片
        /// </summary>
        public virtual string CoverImage { get; set; }
        /// <summary>
        /// 创建人账号
        /// </summary>
        public virtual string CreationAccount { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public virtual string CreationName { get; set; }

        /// <summary>
        /// 项目成员
        /// </summary>
        public virtual ICollection<ProjectUserListDto> ProjectUsers { get; set; }

        /// <summary>
        /// 封面图片（多个）
        /// </summary>        
        public virtual ICollection<ProjectImageListDto> ProjectImages { get; set; }

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
