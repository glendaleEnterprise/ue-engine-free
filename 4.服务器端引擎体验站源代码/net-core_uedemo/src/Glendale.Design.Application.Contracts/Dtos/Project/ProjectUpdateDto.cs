using Glendale.Design.Dtos.ProjectFolder;
using Glendale.Design.Dtos.ProjectImage;
using Glendale.Design.Dtos.ProjectUser;
using Glendale.Design.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Glendale.Design.Dtos.Project
{
    public class ProjectUpdateDto
    {
         
        /// <summary>
        /// 项目名称
        /// </summary>
        [Required, MaxLength(50)]
        public virtual string ProjectName { get; set; }
         
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
        /// 开始日期
        /// </summary>
        public virtual DateTime? BeginDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public virtual DateTime? FinishDate { get; set; }
        /// <summary>
        /// 是否开放
        /// </summary>
        public virtual bool IsPublic { get; set; }
         
        public virtual string[] Tags { get; set; }

        /// <summary>
        /// 封面图片（多个）
        /// </summary>        
        public virtual ICollection<ProjectImageUpdateDto> ProjectImages { get; set; }
        //public virtual ICollection<ProjectImageCreateDto> ProjectImages { get; set; }

        /// <summary>
        /// 项目成员
        /// </summary>
        public virtual ICollection<ProjectUserUpdateDto> ProjectUsers { get; set; }
        //public virtual ICollection<ProjectUserCreateDto> ProjectUsers { get; set; }  

        /// <summary>
        /// 所属机构
        /// </summary>
        public virtual Guid? OrgId { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        public virtual Guid? ManageId { get; set; }

    }
}
