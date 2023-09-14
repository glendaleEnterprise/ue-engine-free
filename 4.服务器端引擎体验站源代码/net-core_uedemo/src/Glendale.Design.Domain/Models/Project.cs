using DocumentFormat.OpenXml.Wordprocessing;
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
    /// 项目
    /// </summary>
    public class Project : FullAuditedEntityWithUser<Guid, IdentityUser>
    {
        #region 属性 
        /// <summary>
        /// 项目名称
        /// </summary>
        [Required, MaxLength(50)]
        public virtual string ProjectName { get; set; }
        /// <summary>
        /// 项目简称
        /// </summary>
        [MaxLength(20)]
        public virtual string ShortName { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [MaxLength(10)]
        public virtual string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [MaxLength(10)]
        public virtual string City { get; set; }
        /// <summary>
        /// 区\县
        /// </summary>
        [MaxLength(10)]
        public virtual string District { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(100)]
        public virtual string Address { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public virtual string Remark { get; set; }
         
        /// <summary>
        /// 立项日期
        /// </summary>
        public virtual DateTime BeginDate { get; set; }

        /// <summary>
        /// 完成日期
        /// </summary>
        public virtual DateTime? FinishDate { get; set; }
         
        /// <summary>
        /// 项目状态
        /// </summary>
        public virtual bool IsPublic { get; set; }

        /// <summary>
        /// 所属类别（多个）
        /// </summary>
        [MaxLength(1000)]
        public virtual string Tags { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public virtual Guid? OrgId { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        public virtual Guid? ManageId { get; set; }
        #endregion

        #region 导航属性

        /// <summary>
        /// 项目图片
        /// </summary>
        [NotMapped]
        public virtual ICollection<ProjectImage> ProjectImages { get; set; }

        /// <summary>
        /// 项目参与用户
        /// </summary>
        [NotMapped]
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }

        /// <summary>
        /// 项目目录
        /// </summary>
        [NotMapped]
        public virtual ICollection<ProjectFolder> ProjectFolders { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public virtual OrganizationUnit OrgInfo { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        public virtual IdentityUser ManageInfo { get; set; }

        #endregion

        public Project() { }

        public Project(Guid id): base(id)
        {

        }
    }
}
