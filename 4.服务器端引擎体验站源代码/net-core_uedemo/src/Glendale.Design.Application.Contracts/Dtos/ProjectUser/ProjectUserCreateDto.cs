using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Glendale.Design.Enums;

namespace Glendale.Design.Dtos.ProjectUser
{
    /// <summary>
    /// 项目用户
    /// </summary>
    public class ProjectUserCreateDto
    {         
        /// <summary>
        /// 用户
        /// </summary>
        [Required]
        public virtual Guid UserId { get; set; }       
        
        public virtual bool IsManager { get; set; }

    }
}
