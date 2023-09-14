using System;
using System.Collections.Generic;
using System.Text;

namespace Glendale.Design.Dtos.User
{
    /// <summary>
    /// 自定义用户Dto
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string RoleType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Describe { get; set; }

    }
}
