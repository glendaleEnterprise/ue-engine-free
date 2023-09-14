using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;

namespace Glendale.Design.Models
{
    public class ActionLog : CreationAuditedEntity<Guid>
    {
        #region 属性
        /// <summary>
        /// 操作名称
        /// </summary>
        [MaxLength(50)]
        public virtual string Name { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        [MaxLength(50)]
        public virtual string ModuleName { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        [MaxLength(500)]
        public virtual string Address { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        [MaxLength(200)]
        public virtual string Content { get; set; }
        #endregion

        #region 导航属性
        [NotMapped]
        public virtual IdentityUser User { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [NotMapped]
        public virtual string UserName => User?.UserName;

        #endregion
    }
}
